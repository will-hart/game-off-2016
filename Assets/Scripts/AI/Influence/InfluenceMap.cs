// /** 
//  * InfluenceMap.cs
//  * Will Hart
//  * 20161102
// */

namespace GameGHJ.AI.Influence
{
    #region Dependencies
    
    using System.Collections.Generic;
    using Unitilities.Tuples;
    using UnityEngine;

    #endregion

    /// <summary>
    /// Represents an influence map and handles the spread of influence throughout the map
    /// </summary>
    public class InfluenceMap : IInfluenceMap
    {
        private readonly object _lockObject = new object();

        private int[,] _backBuffer;
        protected int[,] FrontBuffer;

        protected readonly int WorldX;
        private readonly int _worldY;
        private InfluenceMapSettings _settings;
        private readonly float _decayRate;

        /// <summary>
        /// Sets up the influence map with the given parameters.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="worldSizeX"></param>
        /// <param name="worldSizeY"></param>
        /// <param name="xSize"></param>
        /// <param name="ySize"></param>
        public InfluenceMap(InfluenceMapSettings settings, int worldSizeX, int worldSizeY, int xSize, int ySize)
        {
            FrontBuffer = new int[xSize, ySize];
            _backBuffer = new int[xSize, ySize];

            WorldX = worldSizeX;
            _worldY = worldSizeY;
            MapSizeX = xSize;
            MapSizeY = ySize;
            _settings = settings;

            _decayRate = Mathf.Exp(-1*_settings.Decay);
        }

        /// <summary>
        /// Updates the influence map on a separate thread
        /// </summary>
        public virtual void Update(TerrainMaskMap mask, IEnumerable<InfluenceComp> influences)
        {
            FlipBuffer();
            var visited = SeedInfluence(influences);
            SpreadInfluence(mask, visited);
        }

        /// <summary>
        /// Gets a reference to the current influence map values
        /// </summary>
        public int[,] Map() => FrontBuffer;
        
        /// <summary>
        /// Gets the influence map value at a given world position
        /// </summary>
        /// <param name="worldPos"></param>
        /// <returns></returns>
        public int GetValue(Vector3 worldPos)
        {
            var ipos = ConvertWorldToInfluence(worldPos);
            return FrontBuffer[ipos.first, ipos.second];
        }

        /// <summary>
        /// Uses influence generators to seed the map with the current influence sources
        /// </summary>
        private List<TupleI> SeedInfluence(IEnumerable<InfluenceComp> influences)
        {
            var visited = new List<TupleI>();

            foreach (var inf in influences)
            {
                var pos = inf.Owner.GetComponent<PositionComp>();
                var influencePos = ConvertWorldToInfluence(pos.position);
                
                if (influencePos.first < 0 || influencePos.first >= MapSizeX || 
                    influencePos.second < 0 || influencePos.second >= MapSizeY)
                {
					//Debug.LogWarning($"Influence position exceeds bounds X: {influencePos.first} vs {MapSizeX}, Y: {influencePos.second} vs {MapSizeY}");
					Debug.LogWarning($"Influence position exceeded bounds");
					continue;
                }
                
                lock (_lockObject)
                {
                    var curr = FrontBuffer[influencePos.first, influencePos.second];

                    if (visited.Contains(influencePos))
                    {
                        curr += inf.InfluenceStrength;
                    }
                    else
                    {
                        visited.Add(influencePos);
                        curr = inf.InfluenceStrength;
                    }

                    FrontBuffer[influencePos.first, influencePos.second] = curr;
                }
            }

            return visited;
        }

        /// <summary>
        /// Spreads the influence from the map, taking into account the seeded influence, the 
        /// map settings and also the previous influence values
        /// </summary>
        private void SpreadInfluence(TerrainMaskMap mask, List<TupleI> visited)
        {
            // loop over all influence, setting it to the maximum value of its neighbour's influence
            // including decay.
            lock (_lockObject)
            {
                var maskMap = mask.Map();
                var tup = new TupleI(0, 0); 
                for (var x = 0; x < MapSizeX; ++x)
                {
                    for (var y = 0; y < MapSizeY; ++y)
                    {
                        tup.first = x;
                        tup.second = y;

                        var defaultValue = visited.Contains(tup) ? FrontBuffer[x,y] : 0;
                        FrontBuffer[x, y] = maskMap[x, y] > 0 ? GetBufferValue(x, y, defaultValue) : 0;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the value of the influence map at the given location
        /// </summary>
        /// <param name="x">The x coordinate to examine</param>
        /// <param name="y">The y coordinate to examine</param>
        /// <param name="defaultValue"></param>
        /// <returns>The calculated influence value</returns>
        private int GetBufferValue(int x, int y, int defaultValue)
        {
            // look at the neighbours
            var targetInfluence = defaultValue;
            var maxX = Mathf.Min(MapSizeX - 1, x + 1);
            var maxY = Mathf.Min(MapSizeY - 1, y + 1);
                
            for (var i = Mathf.Max(0, x - 1); i <= maxX; ++i)
            {
                for (var j = Mathf.Max(0, y - 1); j <= maxY; ++j)
                {
                    if (x == i && y == j) continue; // ignore the current cell

                    var newTarget = _backBuffer[i, j]*_decayRate;

                    // diagonals have less influence as they are further away
                    if (x != i && y != j) newTarget *= 0.75f;

                    targetInfluence = Mathf.Max(targetInfluence, Mathf.RoundToInt(newTarget));
                }    
            }
            
            // lerp the value
            var val = Mathf.RoundToInt(Mathf.Lerp(_backBuffer[x, y], targetInfluence, _settings.Momentum));
            return Mathf.Abs(val) < 0.01 ? 0 : val;
        }

        /// <summary>
        /// Flips the existing influence map into the back buffer
        /// </summary>
        private void FlipBuffer()
        {
            lock (_lockObject)
            {
                var t = _backBuffer;
                _backBuffer = FrontBuffer;
                FrontBuffer = t;
            }
        }

        /// <summary>
        /// Converts world coordinates into influence map coordinates as the influence map
        /// is usually lower resolution
        /// </summary>
        /// <param name="worldCoord"></param>
        /// <returns></returns>
        public TupleI ConvertWorldToInfluence(Vector3 worldCoord)
        {
            var x = Mathf.FloorToInt(MapSizeX * worldCoord.x / WorldX);
            var y = Mathf.FloorToInt(MapSizeY * worldCoord.z / _worldY);
            return new TupleI(x, y);
        }

        /// <summary>
        /// Converts influence map coordinates to world coordinates
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public Vector3 ConvertInfluenceToWorld(TupleI source)
        {
            return ConvertInfluenceToWorld(source.first, source.second);
        }

        /// <summary>
        /// Converts influence map coordinates to world coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Vector3 ConvertInfluenceToWorld(int x, int y)
        { 
            return new Vector3(
                WorldX * x / (float)MapSizeX,
                0,
                _worldY * y / (float)MapSizeY
            );
        }

        /// <summary>
        /// Gets the size of the map in the x dimension
        /// </summary>
        public int MapSizeX { get; private set; }

        /// <summary>
        /// Gets the size of the map in the y dimension
        /// </summary>
        public int MapSizeY { get; private set; }
    }
}