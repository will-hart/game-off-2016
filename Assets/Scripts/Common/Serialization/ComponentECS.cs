using System;
using AdvancedInspector;
using FullSerializer;
using GameGHJ.Common.ZenECS;
using UnityEngine;


public abstract class ComponentECS : IJSONSerializable//, ICloneable
{
	public virtual string AssetName { get; }
	public virtual string AssetParentFolder { get; }
	public abstract ComponentTypes ComponentType { get; }

	private Type objectType = null;

	//protected bool isNotOverridden = false;

	public Type ObjectType
	{
		get
		{
			if (objectType == null)
			{
				objectType = this.GetType();
			}
			return objectType;
		}
	}

    [HideInInspector]
	[fsIgnore]
    public Entity Owner { get; set; }

    //public ComponentECS(Entity componentOwner, EntityLoaderData data)
    //{
    //    Owner = componentOwner;
	//
	//	//Don't need to initialize data from Entity Factory, it's serialized from json
	//	if (data != null)
	//		Initialise(data);
    //}

	public ComponentECS()
	{ }

    //protected virtual void Initialise(EntityLoaderData data) { }
	public virtual void Initialise() { }

	//[Inspect("IsNotOverridden", 500)]
	//[Method]
	public virtual void Save()
	{
		JSONSerializer.SaveToJSON(this);
	}

	//[Inspect("IsNotOverridden", 501)]
	//[Method]
	public virtual void Load()
	{
		Clone();
	}

	public virtual bool IsNotOverridden()
	{
		return true;
	}

	public void Clone(ComponentECS jsonToCloneFrom = null)
	{
		//First we create an instance of this specific type.
		
		if (jsonToCloneFrom == null)
			jsonToCloneFrom = JSONSerializer.LoadFromJSON(this);
		ReflectionMethods.Clone(this, jsonToCloneFrom);
		
	}

    public virtual void OnDestroy()
    {
        return;
    }

	/// <summary>
	/// Used by EntityFactory to set owner after entity creation. Can be overridden
	/// by derived components who have child components that also need proper owner init.
	/// See AvailableWeaponsComp as an example of this.
	/// </summary>
	/// <param name="_entity">Entity to set as owner</param>
	public virtual void SetOwner(Entity _entity)
	{
		Owner = _entity;
	}
}
	