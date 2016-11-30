// /** 
//  * InfluenceMapDisplay.cs
//  * Will Hart
//  * 20161109
// */

namespace GameGHJ.Common.Debug
{
    using System;
    using System.Text;

    #region Dependencies

    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using AdvancedInspector;
    using GameGHJ.AI.Influence;
    using UnityEngine;

    #endregion

    public class InfluenceMapDisplay : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private GameObject _influenceMapObject;
        [SerializeField, ReadOnly]private Material _influenceMapMaterial;
        [SerializeField] private List<StrategicAiStateComp> _ais = new List<StrategicAiStateComp>();
        
        [SerializeField] private Color _friendlyColor;
        [SerializeField] private Color _enemyColor;
        [SerializeField] private Color _neutralColor;

        [Range(0, 5), SerializeField] private int _currentInfluenceMapTarget = 2;

        [SerializeField] private bool _autoRange = true;
        [Range(1, 150), SerializeField] private int _maxInfluence = 30;

        [SerializeField, ReadOnly] private string _influenceMapName = "Influence";
        [SerializeField] private bool _showMapInConsole;

        private bool _setup;
        private int _frames;
        private Texture2D _texture;

        private IInfluenceMap _lastMap;

        private void Start()
        {
            if (_influenceMapObject == null) return;
            if (_influenceMapMaterial == null)
            {
                _influenceMapMaterial = _influenceMapObject.GetComponent<MeshRenderer>().sharedMaterial;
            }

            _influenceMapObject.SetActive(false);
            StartCoroutine(InitInfluenceWatcher());
        }

        private void Update()
        {
            ++_frames;
            if (_frames < 60) return;
            _frames = 0;

            _influenceMapObject.SetActive(true);

            GatherAiComponents();

            if (_ais.Count <= 0 || _influenceMapMaterial == null) return;
            if (_ais[0].DecisionSpace == null) return;

            IInfluenceMap map;

            switch (_currentInfluenceMapTarget)
            {
                case 0:
                    map = _ais[0].DecisionSpace.Friendly;
                    _influenceMapName = "Friendly Influence";
                    break;
                case 1:
                    map = _ais[0].DecisionSpace.Enemy;
                    _influenceMapName = "Enemy Influence";
                    break;
                case 2:
                    map = _ais[0].DecisionSpace.Influence;
                    _influenceMapName = "Influence";
                    break;
                case 3:
                    map = _ais[0].DecisionSpace.Scouting;
                    _influenceMapName = "Scouting";
                    break;
                case 4:
                    map = _ais[0].DecisionSpace.Tension;
                    _influenceMapName = "Tension";
                    break;
                case 5:
                    map = _ais[0].DecisionSpace.Vulnerability;
                    _influenceMapName = "Vulnerability";
                    break;

                default:
                    map = _ais[0].DecisionSpace.Influence;
                    _influenceMapName = "Influence";
                    break;
            }

            BuildDebugDisplay(_influenceMapMaterial, map);
            if (_showMapInConsole) ShowMapInConsole(map);
        }

        private static void ShowMapInConsole(IInfluenceMap map)
        {
            var sb = new StringBuilder();
            var infMap = map.Map();

            for (var y = map.MapSizeY - 1; y >= 0; y--)
            {
                for (var x = 0; x < map.MapSizeX; x++)
                {
                    sb.Append($"{infMap[x, y]}   ");
                }

                sb.Append(Environment.NewLine);
            }

            Debug.Log(sb.ToString());
        }

        private void BuildDebugDisplay(Material material, IInfluenceMap influenceMap)
        {
            if (_lastMap != influenceMap || _texture == null)
            {
                _texture = new Texture2D(influenceMap.MapSizeX, influenceMap.MapSizeY)
                {
                    filterMode = FilterMode.Point,
                    wrapMode = TextureWrapMode.Clamp
                };
                _lastMap = influenceMap;
            }
            
            var mapRaw = influenceMap.Map();
            float maxInf = _autoRange ? mapRaw.Cast<int>().Max() : _maxInfluence;
            
            for (var x = 0; x < influenceMap.MapSizeX; x++)
            {
                for (var y = 0; y < influenceMap.MapSizeY; y++)
                {
                    var inf = mapRaw[x, y] / maxInf;
                    var col = inf < 0
                        ? Color.Lerp(_neutralColor, _enemyColor, Mathf.Clamp01(-inf))
                        : Color.Lerp(_neutralColor, _friendlyColor, Mathf.Clamp01(inf));
                    _texture.SetPixel(x, y, col);
                }
            }

            _texture.Apply();
            material.SetTexture("_MainTex", _texture);
        }

        private IEnumerator InitInfluenceWatcher()
        {
            yield return new WaitForSeconds(3);
            GatherAiComponents();
            _setup = true;
        }

        private void GatherAiComponents()
        {
            var comps = ZenBehaviourManager.Instance.Get(ComponentTypes.StrategicAiStateComp).Cast<StrategicAiStateComp>().ToList();

            if (comps.Count != _ais.Count)
            {
                _ais = comps;
            }
        }
#endif
    }
}