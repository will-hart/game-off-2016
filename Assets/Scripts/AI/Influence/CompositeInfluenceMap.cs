// /** 
//  * CompositeInfluenceMap.cs
//  * Will Hart
//  * 20161102
// */

namespace GameGHJ.AI.Influence
{
    #region Dependencies

    using System;
    using System.Collections.Generic;
    using Unitilities.Tuples;
    using UnityEngine;

    #endregion

    /// <summary>
    ///     A class to handle operations on influence maps, for instance adding or substracting two influence maps.
    ///     Its a zenbehaviour to maintain the same interface as InfluenceMap
    /// </summary>
    public class CompositeInfluenceMap : IInfluenceMap
    {
        private readonly int[,] _map;

        private readonly IInfluenceMap _mapA;
        private readonly IInfluenceMap _mapB;
        private readonly Func<int, int> _transformA;
        private readonly Func<int, int> _transformB;
        
        private readonly object _lockObject = new object();
        
        /// <summary>
        ///     Initialises the influence map with simple multiplication transforms
        /// </summary>
        /// <param name="multiplierA"></param>
        /// <param name="mapA"></param>
        /// <param name="multiplierB"></param>
        /// <param name="mapB"></param>
        public CompositeInfluenceMap(int multiplierA, IInfluenceMap mapA, int multiplierB, IInfluenceMap mapB)
            : this(val => multiplierA*val, mapA, val => multiplierB*val, mapB)
        {
        }

        /// <summary>
        ///     Intialises the influence map with a multiplication and a float>>float transform
        /// </summary>
        /// <param name="multiplierA"></param>
        /// <param name="mapA"></param>
        /// <param name="transformB"></param>
        /// <param name="mapB"></param>
        public CompositeInfluenceMap(int multiplierA, IInfluenceMap mapA, Func<int, int> transformB, IInfluenceMap mapB)
            : this(val => multiplierA*val, mapA, transformB, mapB)
        {
        }

        /// <summary>
        ///     Default constructor
        /// </summary>
        /// <param name="transformA"></param>
        /// <param name="mapA"></param>
        /// <param name="transformB"></param>
        /// <param name="mapB"></param>
        private CompositeInfluenceMap(
            Func<int, int> transformA,
            IInfluenceMap mapA,
            Func<int, int> transformB,
            IInfluenceMap mapB)
        {
            if ((mapA.MapSizeX != mapB.MapSizeY) ||
                (mapA.MapSizeY != mapB.MapSizeY))
                throw new ArgumentException("Invalid map sizes in composite influence map");

            _mapA = mapA;
            _mapB = mapB;

            _transformA = transformA;
            _transformB = transformB;

            _map = new int[mapA.MapSizeX, mapB.MapSizeY];
        }
        
        /// <summary>
        ///     Updates the map
        /// </summary>
        public void Update(TerrainMaskMap mask, IEnumerable<InfluenceComp> influences)
        {
            RefreshMap(mask);
        }

        /// <summary>
        ///     Gets the current map value, refreshing if child maps are dirty
        /// </summary>
        /// <returns></returns>
        public int[,] Map()
        {
            return _map;
        }

        /// <summary>
        /// Gets the influence map value at a given world position
        /// </summary>
        /// <param name="worldPos"></param>
        /// <returns></returns>
        public int GetValue(Vector3 worldPos)
        {
            var ipos = _mapA.ConvertWorldToInfluence(worldPos);

            if (ipos.first < MapSizeX && ipos.second < MapSizeY) return _map[ipos.first, ipos.second];

            Debug.LogWarning("Getting value outside influence map bounds");
            return 0;
        }

        public TupleI ConvertWorldToInfluence(Vector3 worldCoord) => _mapA.ConvertWorldToInfluence(worldCoord);
        public Vector3 ConvertInfluenceToWorld(int x, int y) => _mapA.ConvertInfluenceToWorld(x, y);

        public int MapSizeX => _mapA.MapSizeX;
        public int MapSizeY => _mapA.MapSizeY;

        /// <summary>
        ///     Refreshes the map using the current child influence map values
        /// </summary>
        private void RefreshMap(TerrainMaskMap mask)
        {
            lock (_lockObject)
            {
                var mapA = _mapA.Map();
                var mapB = _mapB.Map();

                for (var x = 0; x < _mapA.MapSizeX; ++x)
                {
                    for (var y = 0; y < _mapB.MapSizeY; ++y)
                    {
                        _map[x, y] = _transformA(mapA[x, y]) + _transformB(mapB[x, y]);
                    }
                }
            }
        }
    }
}