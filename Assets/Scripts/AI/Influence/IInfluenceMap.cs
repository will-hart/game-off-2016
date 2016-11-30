// /** 
//  * IInfluenceMap.cs
//  * Will Hart
//  * 20161102
// */

namespace GameGHJ.AI.Influence
{
    using System.Collections.Generic;
    using Unitilities.Tuples;
    using UnityEngine;

    public interface IInfluenceMap
    {
        // NOTE: This is a method rather than property as getting composite influence maps can have side effects
        int[,] Map();
        int GetValue(Vector3 worldPos);
    
        void Update(TerrainMaskMap terrainMask, IEnumerable<InfluenceComp> influence);

        int MapSizeX { get; }
        int MapSizeY { get; }

        TupleI ConvertWorldToInfluence(Vector3 pos);
        Vector3 ConvertInfluenceToWorld(int x, int y);
    }
}