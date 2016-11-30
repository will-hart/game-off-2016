using System;

public interface IZenBehaviour : IExecutionPriority, IEnableable
{
	Guid UniqueID { get; }
	Type ObjectType { get; }
	
}

public interface IEnableable
{
	bool IsEnabled { get; set; }
}

public interface IExecutionPriority
{
	int ExecutionPriority { get; }
}
///OnAwake happens once upon Instantiation, so after Awake and OnEnable
public interface IOnAwake : IExecutionPriority, IEnableable
{
	///OnAwake happens once upon Instantiation, so after Awake and OnEnable
	void OnAwake();
}

///OnStart for a given class happens before its first Update/Fixed Update, but after OnAwake
public interface IOnStart : IExecutionPriority, IEnableable
{
	void OnStart();
}

public interface IOnUpdate : IExecutionPriority, IEnableable
{
	void OnUpdate();
}

public interface IOnFixedUpdate : IExecutionPriority, IEnableable
{
	void OnFixedUpdate();
}

public interface IOnLateUpdate : IExecutionPriority, IEnableable
{
	void OnLateUpdate();
}