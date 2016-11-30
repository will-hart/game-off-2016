#pragma warning disable 0414, 0219, 649, 169, 618, 1570
using FullInspector;
using GameGHJ.Common.Behaviours;
using UnityEngine;

/// Allows viewing in Inspector for debug/dev purposes, serves no in-game function
public class EntityWrapper : BaseBehavior//BaseBehavior 
{
    [fiInspectorOnly]   
    public Entity entity;

    protected override void Awake()
    {
        base.Awake();
        GetComponentInChildren<EnemiesInRangeMonitor>()?.SetWrapper(this);
    }

	public void SetEntity(Entity _entity)
	{
		entity = _entity;
		InjectTransform();
		entity.Wrapper = this;
	}

	void OnDestroy()
	{
		foreach (var comp in entity.Components)
		{
			if (comp != null)
				ZenBehaviourManager.Instance?.DestroyComponent(comp);
		}

		//Destroy(entity.Wrapper.gameObject);
	}

	

	public void InjectTransform()
	{
		entity.GetComponent<PositionComp>().transform = transform;
	}
}
