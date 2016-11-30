using System;

public interface IJSONSerializable
{
	string AssetName { get; }
	string AssetParentFolder { get; }

	Type ObjectType { get; }
    ComponentTypes ComponentType { get; }
    //void SaveToJSON();
    //void LoadFromJSON();
}