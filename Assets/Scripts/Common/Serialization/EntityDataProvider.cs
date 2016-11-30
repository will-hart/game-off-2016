// /** 
//  * EntityDataProvider.cs
//  * Will Hart
//  * 20161106
// */

namespace GameGHJ.Common.Serialization
{
    #region Dependencies

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FullSerializer;
    using UnityEngine;
    
    using ZenECS;

    #endregion

    /// <summary>
    /// A class to load entity data from JSON files in resources and provide them statically to the application
    /// </summary>
    public static class EntityDataProvider
    {
        private const string BuildingPath = "JSON\\Buildings";
        private const string CreepsPath = "JSON\\Creeps";
        private const string HeroesPath = "JSON\\Heroes";

        private static readonly fsSerializer Serializer = new fsSerializer();

        /// <summary>
        /// Static constructor, loads entity data from files
        /// </summary>
        static EntityDataProvider()
        {
            MergeAssets(Buildings, LoadEntityDataType(BuildingPath));
            MergeAssets(Creeps, LoadEntityDataType(CreepsPath));
            MergeAssets(Heroes, LoadEntityDataType(HeroesPath));
        }

        /// <summary>
        /// Gets entity data for a building by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static EntityLoaderData GetBuilding(int id)
        {
            if (!Buildings.ContainsKey(id))
            {
                throw new ArgumentException($"Unknown building id - {id}");
            }

            return Buildings[id];
        }

        /// <summary>
        /// Gets entity data for a building by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static EntityLoaderData GetCreep(int id)
        {
            if (!Creeps.ContainsKey(id))
            {
                throw new ArgumentException($"Unknown creep id - {id}");
            }

            return Creeps[id];
        }

        /// <summary>
        /// Gets entity data for a building by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static EntityLoaderData GetHero(int id)
        {
            if (!Heroes.ContainsKey(id))
            {
                throw new ArgumentException($"Unknown hero id - {id}");
            }

            return Heroes[id];
        }

        /// <summary>
        /// Merge the list of parsed entity data in to the data provider dictionary
        /// </summary>
        /// <param name="assetDict"></param>
        /// <param name="entities"></param>
        private static void MergeAssets(IDictionary<int, EntityLoaderData> assetDict, IEnumerable<EntityLoaderData> entities)
        {
            foreach (var entity in entities)
            {
                if (assetDict.ContainsKey(entity.Id))
                {
                    assetDict[entity.Id] = entity;
                }
                else
                {
                    assetDict.Add(entity.Id, entity);
                }
            }
        }

        /// <summary>
        /// Load all entity data JSON files from a given resources folder
        /// </summary>
        /// <param name="resourcesPath"></param>
        /// <returns></returns>
        private static IEnumerable<EntityLoaderData> LoadEntityDataType(string resourcesPath)
        {
            return Resources.LoadAll<TextAsset>(resourcesPath)
                .Select(res => fsJsonParser.Parse(res.text))
                .Select(
                    fsd =>
                    {
                        object deserialized = null;
                        Serializer.TryDeserialize(fsd, typeof(EntityLoaderData), ref deserialized);
                        return (EntityLoaderData) deserialized;
                    });
        }

        private static Dictionary<int, EntityLoaderData> Buildings => new Dictionary<int, EntityLoaderData>();
        private static Dictionary<int, EntityLoaderData> Creeps => new Dictionary<int, EntityLoaderData>();
        private static Dictionary<int, EntityLoaderData> Heroes => new Dictionary<int, EntityLoaderData>();
    }
}