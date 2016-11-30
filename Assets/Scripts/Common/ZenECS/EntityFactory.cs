#pragma warning disable 0414, 0219, 649, 169, 618, 1570
using System;
using System.Collections.Generic;
using FullSerializer;
using UnityEngine;

public class EntityFactory : ZenSingleton<EntityFactory>, IOnAwake
{
	private readonly fsSerializer Serializer = new fsSerializer();
	private fsData data;

	private Dictionary<string, fsData> EntityTemplates = new Dictionary<string, fsData>();

	public override int ExecutionPriority => -1000;
	public override Type ObjectType => typeof(EntityFactory);

	public void OnAwake()
	{
		PreparseJSON();
	}

	private void PreparseJSON()
	{
		List<string> resourcePaths = new List<string>
		{
			"JSON/Buildings",
			"JSON/Creeps",
			"JSON/Heroes"
		};
		
		foreach (var filePath in resourcePaths)
		{
			var res = LoadAllResources(filePath);

			foreach (var asset in res)
			{
				fsData fsdata = fsJsonParser.Parse(asset.text);

				var fsdict = fsdata.AsDictionary;
				
				if (fsdict.ContainsKey("EntityName"))
				{
					var entname = fsdict["EntityName"].AsString;
					EntityTemplates.Add(entname, fsdata);
				}
			}
		}
	}

	public GameObject CreateGameObjectFromTemplate(string _EntityName, Transform parent = null)
	{
		//#TODO: Switch this to using pooling
		GameObject go;

		//go.AddComponent<EntityWrapper>();
		//go.GetComponent<EntityWrapper>().SetEntity(CreateEntityFromTemplate(_EntityName));
		
		//Debug prefab attach
		Entity newEntity = CreateEntityFromTemplate(_EntityName);

        if (newEntity.HasComponent(ComponentTypes.UnityPrefabComp))
        {
            var pc = newEntity.GetComponent<UnityPrefabComp>();
            var prefab = ResourcesManager.PrefabStorage[pc.prefabType];

            go = prefab.InstantiateFromPool();

        }
        else
		{
			go = new GameObject(_EntityName);
		}

		go.AddComponent<EntityWrapper>();
		go.GetComponent<EntityWrapper>().SetEntity(newEntity);

		if (parent != null)
			go.transform.SetParent(parent);

		InitializeGameObject(go);
		return go;
	}

	

	public Entity CreateEntityFromTemplate(string _EntityName)
	{
		object deserialized = null;

		Serializer.TryDeserialize(
			EntityTemplates[_EntityName],
			typeof(Entity),
			ref deserialized
		);

		Entity newEntity =  (Entity) deserialized;
		InitializeNewEntity(newEntity);

		return newEntity;
	}

    public fsData GetTemplate(string entityName)
    {
        return !EntityTemplates.ContainsKey(entityName) ? null : EntityTemplates[entityName];
    }

	/// <summary>
	/// Load all entity data JSON files from a given resources folder
	/// </summary>
	/// <param name="resourcesPath"></param>
	/// <returns></returns>
	private  TextAsset[] LoadAllResources(string resourcesPath)
	{
		return Resources.LoadAll<TextAsset>(resourcesPath);
	}

	private void InitializeNewEntity(Entity ent)
	{
		foreach (var cmp in ent.Components)
		{
			cmp.SetOwner(ent);
			cmp.Initialise();
			ZenBehaviourManager.Instance.AddComponent(cmp);
		}

		if (ent.HasComponent(ComponentTypes.CombatComp))
		{
			ent.GetComponent<CombatComp>().selectedWeapon = ent.GetComponent<AvailableWeaponsComp>().availableWeaponsList[0];
		}
	}

	private void InitializeGameObject(GameObject go)
	{
		var ew = go.GetComponent<EntityWrapper>();

		//Add audio source unity component
		if (ew.entity.HasComponent(ComponentTypes.AudioSourceComp))
		{
			var audioSrc = go.AddComponent<AudioSource>();
			audioSrc.playOnAwake = false;
			ew.entity.GetComponent<AudioSourceComp>().audioSource = audioSrc;
		}

		// Add ranged projectile components
		if (ew.entity.HasComponent(ComponentTypes.MissileComp))
		{
			ew.entity.GetComponent<MissileComp>().InitializeGameObject();
		}
	}
}
