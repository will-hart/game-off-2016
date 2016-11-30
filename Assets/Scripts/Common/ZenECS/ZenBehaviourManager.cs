#pragma warning disable 0414, 0219, 649, 169, 618, 1570
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct ZenImplementationFlags
{
	public bool HasOnAwake;
	public bool HasOnStart;
	public bool HasOnUpdate;
	public bool HasOnFixedUpdate;
	public bool HasOnLateUpdate;
}

///Instantiate runs Awake and OnEnable but not Start
public class ZenBehaviourManager : MonoSingleton<ZenBehaviourManager>
{
	private List<IOnStart> OnStartList = new List<IOnStart>();
	private List<IOnStart> StartToRemoveList = new List<IOnStart>();
	private List<IOnUpdate> OnUpdateList = new List<IOnUpdate>();
	private List<IOnFixedUpdate> OnFixedUpdateList = new List<IOnFixedUpdate>();
	private List<IOnLateUpdate> OnLateUpdateList = new List<IOnLateUpdate>();
	private Dictionary<Type, ZenImplementationFlags> typeImplDict = new Dictionary<Type, ZenImplementationFlags>();

    [SerializeField]
    private Dictionary<ComponentTypes, List<ComponentECS>> ComponentPools =
        new Dictionary<ComponentTypes, List<ComponentECS>>(Enum.GetNames(typeof(ComponentTypes)).Length);


    public void AddComponent(ComponentECS component)
    {
        try
        {
            ComponentPools[component.ComponentType].Add(component);
        }
        catch (KeyNotFoundException k)
        {
            Debug.Log(k);
        }
    }

    public IEnumerable<ComponentECS> Get(ComponentTypes type)
    {
        return ComponentPools[type];
    }

    public IEnumerable<T> Get<T>(ComponentTypes type) where T : ComponentECS
    {
        return ComponentPools[type].Cast<T>();
    }
    
    protected void Awake()
    {
        Reset();
    }

    public void Reset()
    {
        foreach (var componentList in ComponentPools.Values)
        {
            var tempList = new List<ComponentECS>(componentList);
            foreach (var component in tempList)
            {
                DestroyComponent(component);
            }
        }

        ComponentPools.Clear();

        var enumVals = Enum.GetValues(typeof(ComponentTypes));

        foreach (var ev in enumVals)
        {
            var ct = (ComponentTypes)ev;
            ComponentPools.Add(ct, new List<ComponentECS>());
        }
    }

    public void DestroyComponent(ComponentECS component)
    {
        component.OnDestroy();
        ComponentPools[component.ComponentType].Remove(component);
    }

    public void RegisterZenBehaviour(IZenBehaviour newZenBehaviour)
	{
		ZenImplementationFlags currFlags;
		if (typeImplDict.ContainsKey(newZenBehaviour.ObjectType))
		{
			//already loaded this type, add to lists via impl flags
			typeImplDict.TryGetValue(newZenBehaviour.ObjectType, out currFlags);
		}
		else
		{
			//First encounter with this type
			currFlags.HasOnAwake = newZenBehaviour is IOnAwake;
			currFlags.HasOnStart = newZenBehaviour is IOnStart;
			currFlags.HasOnUpdate = newZenBehaviour is IOnUpdate;
			currFlags.HasOnFixedUpdate = newZenBehaviour is IOnFixedUpdate;
			currFlags.HasOnLateUpdate = newZenBehaviour is IOnLateUpdate;
			typeImplDict.Add(newZenBehaviour.ObjectType, currFlags);
		}
		
		if (currFlags.HasOnStart)
		{
			OnStartList.Add((IOnStart)newZenBehaviour);
			OnStartList.Sort((a, b) => a.ExecutionPriority.CompareTo(b.ExecutionPriority));
		}
		if (currFlags.HasOnUpdate)
		{
			OnUpdateList.Add((IOnUpdate)newZenBehaviour);
			OnUpdateList.Sort((a, b) => a.ExecutionPriority.CompareTo(b.ExecutionPriority));
		}
		if (currFlags.HasOnFixedUpdate)
		{
			OnFixedUpdateList.Add((IOnFixedUpdate)newZenBehaviour);
			OnFixedUpdateList.Sort((a, b) => a.ExecutionPriority.CompareTo(b.ExecutionPriority));
		}
		if (currFlags.HasOnLateUpdate)
		{
			OnLateUpdateList.Add((IOnLateUpdate)newZenBehaviour);
			OnLateUpdateList.Sort((a, b) => a.ExecutionPriority.CompareTo(b.ExecutionPriority));
		}
		
		if(currFlags.HasOnAwake) ((IOnAwake)newZenBehaviour).OnAwake();
	}

	public void DeregisterZenBehaviour(IZenBehaviour removedZenBehaviour)
	{
		ZenImplementationFlags currFlags;
		
		//already loaded this type, add to lists via impl flags
		typeImplDict.TryGetValue(removedZenBehaviour.ObjectType, out currFlags);
		
		if (currFlags.HasOnStart)
		{
			OnStartList.Remove((IOnStart)removedZenBehaviour);
		}
		if (currFlags.HasOnUpdate)
		{
			OnUpdateList.Remove((IOnUpdate)removedZenBehaviour);
		}
		if (currFlags.HasOnFixedUpdate)
		{
			OnFixedUpdateList.Remove((IOnFixedUpdate)removedZenBehaviour);
		}
		if (currFlags.HasOnLateUpdate)
		{
			OnLateUpdateList.Remove((IOnLateUpdate)removedZenBehaviour);
		}
	}
	
	void ProcessAwaitingStart()
	{
		if (OnStartList.Count <= 0) return;

		StartToRemoveList.Clear();

		for (int i = 0; i < OnStartList.Count; i++)
		{
			StartToRemoveList.Add(OnStartList[i]);
		}

		for (int i = 0; i < StartToRemoveList.Count; i++)
		{
			var idx = StartToRemoveList[i];
			if (idx.IsEnabled)
			{
				idx.OnStart();
				//Debug.Log($"Removing: {idx.GetType().Name}");
				OnStartList.Remove(idx);
			}
		}

	}

	void Update()
	{
		ProcessAwaitingStart();
		for (int i = 0; i < OnUpdateList.Count; i++)
		{
			var idx = OnUpdateList[i];
			if (idx.IsEnabled)
			{
				OnUpdateList[i].OnUpdate();
			}
		}
	}

	void FixedUpdate()
	{
		ProcessAwaitingStart();
		for (int i = 0; i < OnFixedUpdateList.Count; i++)
		{
			var idx = OnFixedUpdateList[i];
			if (idx.IsEnabled)
			{
				idx.OnFixedUpdate();
			}
		}
	}

	void LateUpdate()
	{
		for (int i = 0; i < OnLateUpdateList.Count; i++)
		{
			var idx = OnLateUpdateList[i];
			if (idx.IsEnabled)
			{
				idx.OnLateUpdate();
			}
		}
	}
}
