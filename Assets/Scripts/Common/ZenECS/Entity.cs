using System;
using System.Collections.Generic;
using FullInspector;
using GameGHJ.Common.ZenECS;
using UnityEngine;

public class Entity
{
    [ShowInInspector]
	[SerializeField]
    private Dictionary<Type, ComponentECS> _components;// = new Dictionary<Type, ComponentECS>();

	public string EntityName;

	public EntityWrapper Wrapper { get; set; }

    public Entity()
    {
        //debug purposes
         _components = new Dictionary<Type, ComponentECS>();
    }

	public Entity(string _EntityName)
	{
		//debug purposes
		_components = new Dictionary<Type, ComponentECS>();
		EntityName = _EntityName;
	}

	public void InitializeComponents(List<ComponentTypes> componentTypes, EntityLoaderData data)
    {
        foreach (var component in componentTypes)
        {
            AddComponent(component, data);
        }
    }
    
    public bool HasComponent(ComponentTypes type) => _components.ContainsKey(ComponentFactory.ComponentLookup[type]);

    public T GetComponent<T>() where T : ComponentECS => (T)_components[typeof(T)];

    public void AddComponent(ComponentTypes componentType, EntityLoaderData data)
    {
        var comp = ComponentFactory.Create(componentType, this, data);
        AddComponent(comp);
    }

    public void AddComponent(ComponentECS component)
    {
        _components.Add(component.ObjectType, component);
		if (ZenBehaviourManager.Instance != null)
			ZenBehaviourManager.Instance.AddComponent(component);
    }

    public IEnumerable<ComponentECS> Components => _components.Values;
}
