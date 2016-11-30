// /** 
//  * InfluenceMapExtensions.cs
//  * Will Hart
//  * 20161102
// */

namespace GameGHJ.AI.Influence
{
    #region Dependencies

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniRx;
    using UnityEngine;

    #endregion

    public static class InfluenceMapExtensions
    {
        /// <summary>
        /// Calculates the peak value on the graph and returns the location
        /// Currently uses a brute force search.
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static Vector3 GetPeak(this IInfluenceMap map)
        {
            var m = map.Map();

            var max = 0;
            var maxX = -1;
            var maxY = -1;

            for (var x = 0; x < map.MapSizeX; ++x)
            {
                for (var y = 0; y < map.MapSizeY; ++y)
                {
                    var val = Mathf.Abs(m[x, y]);
                    if (val <= max) continue;

                    max = val;
                    maxX = x;
                    maxY = y;
                }
            }

            return new Vector3(maxX, 0, maxY);
        }

        /// <summary>
        /// Gets data about the influence map such as the number of positive cells and their total strength.
        /// Used by the strategic AI for decision making.
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static InfluenceMapBalance GetBalance(this IInfluenceMap map)
        {
            var result = new InfluenceMapBalance();
            var mapData = map.Map();

            for (var x = 0; x < map.MapSizeX; ++x)
            {
                for (var y = 0; y < map.MapSizeY; ++y)
                {
                    var val = mapData[x, y];
                    if (val > 0)
                    {
                        ++result.PositiveCount;
                        result.PositiveSum += val;
                    }
                    else if (val < 0)
                    {
                        ++result.NegativeCount;
                        result.NegativeSum -= val;
                    }
                    else
                    {
                        ++result.ZeroCount;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Returns lists of positive and negative peaks in the influence map. A peak is defined as a point
        /// which is surrounded by lower or equal values (higher or equal in the case of a negative point).
        /// </summary>
        /// <param name="map"></param>
        /// <returns>A tuple of List of Vector2. The first item is positive peaks, the second is negative peaks</returns>
        public static PeakResults GetPeaks(this IInfluenceMap map)
        {
            var mapData = map.Map();
            var visited = new bool[map.MapSizeX, map.MapSizeY];
            var result = new PeakResults
            {
                Positive = new List<Vector2>(),
                Negative = new List<Vector2>()
            };
            
            // NOTE peaks solely on the edges of the influence map are ignored
            for (var x = 1; x < map.MapSizeX - 1; ++x)
            {
                for (var y = 1; y < map.MapSizeY - 1; ++y)
                {
                    if (visited[x, y]) continue;
                    FillPeak(result, mapData, ref visited, map.MapSizeX, map.MapSizeY, x, y);
                }
            }

            return result;
        }

        /// <summary>
        /// Flood fills from the current point, ignoring visited points and returns the centroid of the given peak
        /// </summary>
        /// <param name="peaks"></param>
        /// <param name="map"></param>
        /// <param name="visited"></param>
        /// <param name="xLim"></param>
        /// <param name="yLim"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private static void FillPeak(PeakResults peaks, int[,] map, ref bool[,] visited, int xLim, int yLim, int x, int y)
        {
            // check the sign of the peak
            var peakType = GetPeakType(map, x, y);
            if (peakType == PeakType.None)
            {
                visited[x, y] = true;
                return;
            }

            var unvisitedPts = new Queue<Vector2>();
            unvisitedPts.Enqueue(new Vector2(x, y));

            var thisPeak = new List<Vector2>();

            while (unvisitedPts.Count > 0)
            {
                var pt = unvisitedPts.Dequeue();
                var intX = (int) pt.x;
                var intY = (int) pt.y;

                if (visited[intX, intY]) continue;
                visited[intX, intY] = true;

                if (GetPeakType(map, intX, intY) != peakType) continue;

                // add this point to the peak
                thisPeak.Add(pt);

                // add adjacent points, ignoring points on the edge of the influence map
                if (!visited[intX - 1, intY] && intX > 1) unvisitedPts.Enqueue(new Vector2(intX - 1, intY    ));
                if (!visited[intX, intY - 1] && intY > 1) unvisitedPts.Enqueue(new Vector2(intX,     intY - 1));
                if (!visited[intX + 1, intY] && intX < xLim - 2) unvisitedPts.Enqueue(new Vector2(intX + 1, intY    ));
                if (!visited[intX, intY + 1] && intY < yLim - 2) unvisitedPts.Enqueue(new Vector2(intX,     intY + 1));
            }

            var sumPt = thisPeak.Aggregate(Vector2.zero, (item, acc) => acc + item);

            if (peakType == PeakType.Negative)
            {
                peaks.Negative.Add(new Vector2((int) (sumPt.x/thisPeak.Count), (int) (sumPt.y/thisPeak.Count)));
            }
            else if (peakType == PeakType.Positive)
            {
                peaks.Positive.Add(new Vector2((int) (sumPt.x/thisPeak.Count), (int) (sumPt.y/thisPeak.Count)));
            }
        }

        /// <summary>
        /// Determine if a given position is a positive peak, negative peak, or not a peak at all
        /// </summary>
        /// <param name="mapData"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static PeakType GetPeakType(int[,] mapData, int x, int y)
        {
            var val = mapData[x, y];

            if (val == 0) return PeakType.None;
            
            var a = mapData[x - 1, y];
            var b = mapData[x, y - 1];
            var c = mapData[x + 1, y];
            var d = mapData[x, y + 1];

            // handle negative peak
            if (val < 0
                && a >= val
                && b >= val
                && c >= val
                && d >= val) return PeakType.Negative;

            // handle positive peak
            else if (val > 0
                && a <= val
                && b <= val
                && c <= val
                && d <= val) return PeakType.Positive;

            return PeakType.None;
        }
        
        private enum PeakType
        {
            Negative,
            None,
            Positive
        }
    }

    public struct PeakResults
    {
        public List<Vector2> Positive;
        public List<Vector2> Negative;
    }
}