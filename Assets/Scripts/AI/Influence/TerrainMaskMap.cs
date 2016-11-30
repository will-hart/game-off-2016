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

    public class TerrainMaskMap : InfluenceMap
    {
        private readonly object _lockObject = new object();

        public TerrainMaskMap(InfluenceMapSettings settings, int worldSizeX, int worldSizeY, int sizeX, int sizeY)
            : base(settings, worldSizeX, worldSizeY, sizeX, sizeY)
        {
        }

        /// <summary>
        ///     Updates the scouting map
        ///     TODO set "sighted" zeroes based on the vision range of units rather than just adjacent cells
        /// </summary>
        public override void Update(TerrainMaskMap mask, IEnumerable<InfluenceComp> influences)
        {
            RefreshTerrainMap();
        }

        /// <summary>
        /// Loop through all terrain items and work out where the influence map is passable
        /// </summary>
        private void RefreshTerrainMap()
        {
            for (int x = 0; x < MapSizeX; x++)
            {
                for (int y = 0; y < MapSizeY; y++)
                {
                    var pos = ConvertInfluenceToWorld(x, y);
                    FrontBuffer[x, y] = IsWalkable(pos) ? 1 : 0;
                }
            }
        }

        /// <summary>
        /// Determine if the cell is walkable
        /// </summary>
        /// <param name="pos"></param>
        private bool IsWalkable(Vector3 pos)
        {
            // get 1/4 of the world tile size so we check at 25% and 75% of tile width/height
            var tileDelta = WorldX/(float) MapSizeX/4f;

            return AstarPath.active.GetNearest(pos).node.Walkable
                   || AstarPath.active.GetNearest(new Vector3(pos.x - tileDelta, pos.y, pos.z - tileDelta)).node.Walkable
                   || AstarPath.active.GetNearest(new Vector3(pos.x - tileDelta, pos.y, pos.z + tileDelta)).node.Walkable
                   || AstarPath.active.GetNearest(new Vector3(pos.x + tileDelta, pos.y, pos.z - tileDelta)).node.Walkable
                   || AstarPath.active.GetNearest(new Vector3(pos.x + tileDelta, pos.y, pos.z + tileDelta)).node.Walkable;
        }
    }
}