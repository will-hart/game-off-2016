// /** 
//  * EnemiesInRangeMonitor.cs
//  * Will Hart
//  * 20161106
// */

namespace GameGHJ.Common.Behaviours
{
    #region Dependencies

    using System.Collections;
    using UnityEngine;

    #endregion
    
    public class EnemiesInRangeMonitor : MonoBehaviour
    {
        [SerializeField] private EntityWrapper _entityWrapper;

        private CombatComp _combatComp;
        private UnitPropertiesComp _unitPropsComp;
        private PositionComp _posComp;

        /// <summary>
        /// Sets the entity wrapper to monitor and runs set up one frame later
        /// 
        /// NOTE entity wrapper is stored rather than entity so we don't have to worry about init execution order
        /// </summary>
        /// <param name="wrapper"></param>
        public void SetWrapper(EntityWrapper wrapper)
        {
            _entityWrapper = wrapper;
			StartCoroutine(CheckEntityDelay());
        }

        private void Update()
        {
            _combatComp?.EnemiesInRange.RemoveAll(eir => eir == null || eir.transform == null);
        }

        /// <summary>
        /// Checks that an entity has been correctly setup, and if it has then initialises the collider
        /// </summary>
        /// <returns></returns>
        private IEnumerator CheckEntityDelay()
        {
            yield return new WaitForSeconds(0.2f);
			CheckEntity();
        }

	    private void CheckEntity()
	    {
	        if (_entityWrapper == null || _entityWrapper.entity == null)
	        {
	            gameObject.SetActive(false);
	            return;
	        }

            GetComponent<SphereCollider>().enabled = true;
            _unitPropsComp = _entityWrapper.entity.GetComponent<UnitPropertiesComp>();
            _posComp = _entityWrapper.entity.GetComponent<PositionComp>();

            if (!_entityWrapper.entity.HasComponent(ComponentTypes.CombatComp))
	        {
	            enabled = false;
	            return;
	        }

			_combatComp = _entityWrapper.entity.GetComponent<CombatComp>();
		}

        private void OnTriggerEnter(Collider other)
        {
            
            var monitor = other.GetComponent<EnemiesInRangeMonitor>();
            if (monitor == null) return; // didn't hit an entity
            
            var pos = monitor.Owner.GetComponent<PositionComp>();
            var props = monitor.Owner.GetComponent<UnitPropertiesComp>();
            if (pos == null || props == null) return; // entity isn't attackable
            
			if (props.teamID <= 0 || props.teamID == _unitPropsComp.teamID) return; // entity is neutral or on same team

            if (_combatComp == null || _combatComp.EnemiesInRange.Contains(pos)) return; // don't add more than once
            _combatComp.EnemiesInRange.Add(pos);
        }

        private void OnTriggerExit(Collider other)
        {
            var monitor = other.GetComponent<EnemiesInRangeMonitor>();
            if (monitor == null) return; // didn't hit an entity
            
            var pos = monitor.Owner.GetComponent<PositionComp>();
            if (pos == null) return; // entity isn't attackable

            _combatComp?.EnemiesInRange.Remove(pos);
        }

        private Entity Owner => _entityWrapper.entity;
    }
}