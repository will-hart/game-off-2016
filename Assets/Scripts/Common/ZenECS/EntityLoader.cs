// /** 
//  * EntityLoader.cs
//  * Will Hart
//  * 20161105
// */

namespace GameGHJ.Common.ZenECS
{
    #region Dependencies

    using System;
    using UnityEngine;

    #endregion

    public class EntityLoader : ZenBehaviour, IOnStart
    {
        public EntityLoaderData Data;

        public void OnStart()
        {
            Debug.Log("Starting Entity Loader");

            var entity = new Entity();
            entity.InitializeComponents(Data.Components, Data);

            if (entity.HasComponent(ComponentTypes.PositionComp))
            {
                entity.GetComponent<PositionComp>().transform = transform;
                transform.position = Data.Position_Position;
                transform.rotation = Data.Position_Rotation;
            }

#if UNITY_EDITOR
			GetComponent<EntityWrapper>().entity = entity;
#endif
		}

	    public override int ExecutionPriority => 0;

        public override Type ObjectType => typeof(EntityLoader);
    }
}