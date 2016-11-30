// /** 
//  * StrategicDecisionSpace.cs
//  * Will Hart
//  * 20161102
// */

namespace GameGHJ.AI.Strategy
{
    #region Dependencies
    
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GameGHJ.AI.Influence;
    using UnityEngine;

    #endregion

    /// <summary>
    ///     A class which contains all the information the strategic AI needs to make decisions
    /// </summary>
    public class StrategicDecisionSpace
    {
        private readonly int _enemyTeamId;
        private readonly int _friendlyTeamId;

        public StrategicDecisionSpace(
            int friendlyTeamId,
            int enemyTeamId,
            int worldSizeX,
            int worldSizeY,
            int mapDimension = 128)
        {
            _friendlyTeamId = friendlyTeamId;
            _enemyTeamId = enemyTeamId;

            var settings = new InfluenceMapSettings
                {
                    Decay = 0.2f,
                    Momentum = 0.6f
                };

            Friendly = new InfluenceMap(
                settings,
                worldSizeX,
                worldSizeY,
                mapDimension,
                mapDimension);

            Enemy = new InfluenceMap(
                settings,
                worldSizeX,
                worldSizeY,
                mapDimension,
                mapDimension);

            TerrainMask = new TerrainMaskMap(
                settings,
                worldSizeX,
                worldSizeY,
                mapDimension,
                mapDimension);

            Influence = new CompositeInfluenceMap(1, Friendly, -1, Enemy);
            Tension = new CompositeInfluenceMap(1, Friendly, 1, Enemy);
            Vulnerability = new CompositeInfluenceMap(1, Tension, i => -Mathf.Abs(i), Influence);

            Scouting = new ScoutingInfluenceMap(
                new InfluenceMapSettings
                {
                    Decay = 0,
                    Momentum = 0
                },
                worldSizeX,
                worldSizeY,
                mapDimension,
                mapDimension);
        }

        public IInfluenceMap Friendly { get; }
        public IInfluenceMap Enemy { get; }
        public IInfluenceMap Influence { get; }
        public IInfluenceMap Vulnerability { get; }
        public IInfluenceMap Scouting { get; }
        public IInfluenceMap Tension { get; }
        public TerrainMaskMap TerrainMask { get; }

        public void Update(List<InfluenceComp> influences)
        {
            TerrainMask.Update(null, null);

            var friendly =
                influences.Where(inf => inf.Owner.GetComponent<UnitPropertiesComp>().teamID == _friendlyTeamId).ToList();

            var enemy =
                influences.Where(
                    inf =>
                    {
                        var teamId = inf.Owner.GetComponent<UnitPropertiesComp>().teamID;
                        return teamId > 0 && teamId == _enemyTeamId;
                    });

            
            var friendlyMap = Task.Factory.StartNew(() => Friendly.Update(TerrainMask, friendly));
            var enemyMap = Task.Factory.StartNew(() => Enemy.Update(TerrainMask, enemy));
            Task.Factory.StartNew(() => Scouting.Update(TerrainMask, friendly));

            Task.Factory.ContinueWhenAll(
                new[] {friendlyMap, enemyMap},
                tasks =>
                {
                    Influence.Update(TerrainMask, null);
                    Vulnerability.Update(TerrainMask, null);
                    Tension.Update(TerrainMask, null);
                });

        }
    }
}