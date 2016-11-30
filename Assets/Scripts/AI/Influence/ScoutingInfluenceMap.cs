// /** 
//  * ScoutingInfluenceMap.cs
//  * Will Hart
//  * 20161102
// */

namespace GameGHJ.AI.Influence
{
    #region Dependencies

    using System.Collections.Generic;
    using UnityEngine;

    #endregion

    public class ScoutingInfluenceMap : InfluenceMap
    {
        private readonly object _lockObject = new object();

        public ScoutingInfluenceMap(InfluenceMapSettings settings, int worldSizeX, int worldSizeY, int sizeX, int sizeY)
            : base(settings, worldSizeX, worldSizeY, sizeX, sizeY)
        {
        }

        /// <summary>
        ///     Updates the scouting map
        ///     TODO set "sighted" zeroes based on the vision range of units rather than just adjacent cells
        /// </summary>
        public override void Update(TerrainMaskMap mask, IEnumerable<InfluenceComp> influences)
        {
            IncrementGridSightValues();

            foreach (var influence in influences)
            {
                UpdateInfluenceSightValues(influence);
            }
        }

        private void IncrementGridSightValues()
        {
            lock (_lockObject)
            {
                for (var x = 0; x < MapSizeX; ++x)
                {
                    for (var y = 0; y < MapSizeY; ++y)
                    {
                        FrontBuffer[x, y] += 1;
                    }
                }
            }
        }

        private void UpdateInfluenceSightValues(InfluenceComp influence)
        {
            var pos = influence.Owner.GetComponent<PositionComp>().position;
            var influencePos = ConvertWorldToInfluence(pos);
            var sightRange = Mathf.RoundToInt(influence.ScoutVisionRange * MapSizeX / (float)WorldX);

            var x = influencePos.first;
            var y = influencePos.second;

            // otherwise look at the neighbours
            var radSqr = sightRange*sightRange;

            // see https://jsperf.com/point-in-circle/2
            for (var i = Mathf.Max(0, x - sightRange); i <= x; ++i)
            {
                for (var j = Mathf.Max(0, y - sightRange); j <= y; ++j)
                {
                    if ((i - x)*(i - x) + (j - y)*(j - y) > radSqr) continue;

                    var xSym = x - (i - x);
                    var ySym = y - (j - y);

                    lock (_lockObject)
                    {
                        FrontBuffer[i, j] = 0;

                        if (ySym < MapSizeY)
                        {
                            FrontBuffer[i, ySym] = 0;
                            if (xSym < MapSizeX) FrontBuffer[xSym, ySym] = 0;
                        }

                        if (xSym < MapSizeX) FrontBuffer[xSym, j] = 0;
                    }
                }
            }
        }
    }
}