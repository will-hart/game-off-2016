using System;
using System.Collections.Generic;
using System.IO;
using AdvancedInspector;
using FullInspector;
using FullSerializer;
using GameGHJ.Common.ZenECS;
using UnityEngine;

[ExecuteInEditMode]
public class EntityGUICreator : MonoBehaviour
{
    //#TODO: Add name field to entity, cache clones as template
	private Entity newEnt = new Entity();

	[Inspect(-500)] [NonSerialized] public Dictionary<ComponentTypes, List<ComponentECS>> ComponentPools =
		new Dictionary<ComponentTypes, List<ComponentECS>>(Enum.GetNames(typeof(ComponentTypes)).Length);

	[Inspect(0)] public ComponentTypes typeToCreate;

	[Inspect(20)]
	[Method]
	public void AddChosenComponent()
	{
		Debug.Log("Add");
		var lookup = ComponentFactory.ComponentLookup;
		var newtype = ComponentFactory.ComponentLookup[typeToCreate];
		//EntityLoaderData edata = new EntityLoaderData(new List<ComponentTypes>() {typeToCreate});

		if (!ComponentPools.ContainsKey(typeToCreate))
		{
			ComponentPools[typeToCreate] = new List<ComponentECS>();
		}
		ComponentPools[typeToCreate].Add(
			//(ComponentECS) Activator.CreateInstance(ComponentFactory.ComponentLookup[typeToCreate], newEnt, edata));
			(ComponentECS)Activator.CreateInstance(ComponentFactory.ComponentLookup[typeToCreate]));


	}

	[Inspect(50)] public int EntityID;
	[Inspect(60)] public string EntityName;
	[Inspect(75)] public EntityType entityType;

	private static readonly fsSerializer _serializer = new fsSerializer();
	private static fsData data;

	[Inspect(100)]
	[Method]
	public void SaveComponentToJSON()
	{
		newEnt = new Entity(EntityName);
		foreach (var typ in ComponentPools)
		{
			foreach (var cmp in typ.Value)
				newEnt.AddComponent(cmp);
		}

		_serializer.TrySerialize(typeof(Entity), newEnt, out data).AssertSuccess();

		string filePath = Application.dataPath + "/Resources/JSON/" + entityType.ToString() + "/" + EntityName +".json";

		Debug.Log("Saving to: " + filePath);
		Directory.CreateDirectory(Application.dataPath + "/Resources/JSON/" + entityType.ToString() + "/");

		using (FileStream file = File.Open(filePath, FileMode.Create))
		{
			using (StreamWriter writer = new StreamWriter(file))
			{
				fsJsonPrinter.PrettyJson(data, writer);
			}
		}
	}

	[Inspect(200)]
	[Method]
	public void LoadFromJSON()
	{
		Reset();
		string filePath = Application.dataPath + "/Resources/JSON/" + entityType.ToString() + "/" + EntityName + ".json";

		if (!File.Exists(filePath))
		{
			Debug.Log("No existing JSON file");

		}
		using (StreamReader reader = new StreamReader(filePath))
		{
			string strdata = reader.ReadToEnd();
			fsJsonParser.Parse(strdata, out data);
			object deserialized = null;
			_serializer.TryDeserialize(data, typeof(Entity), ref deserialized).AssertSuccessWithoutWarnings();

			// wtf pretty much describes it, FS decides to create a new game object, this gets rid of it
			GameObject wtf = GameObject.Find("New Game Object");
			if (wtf != null)
				DestroyImmediate(wtf);
		
			if (deserialized != null)
			{
				//return deserialized as Entity;
				newEnt = (Entity) deserialized;
				foreach (var cmp in newEnt.Components)
				{
					if (!ComponentPools.ContainsKey(cmp.ComponentType))
					{
						ComponentPools[cmp.ComponentType] = new List<ComponentECS>();
					}
					ComponentPools[cmp.ComponentType].Add(cmp);
				}
			}
			else 
			{
				Debug.Log("Not deserialized");
			}
		}
	}

	[Inspect(250)]
	[Method]
	public void Reset()
	{
		ComponentPools.Clear();
		newEnt = new Entity();
	}
}


public enum EntityType
{
	Buildings,
	Creeps,
	Heroes
}

//private const string BuildingPath = "JSON\\Buildings";
//private const string CreepsPath = "JSON\\Creeps";
//private const string HeroesPath = "JSON\\Heroes";