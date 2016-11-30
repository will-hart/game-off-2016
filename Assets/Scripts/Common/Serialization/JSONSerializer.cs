#pragma warning disable 0414, 0219, 649, 169, 618, 1570
using System;
using UnityEngine;
using System.IO;
using FullSerializer;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class JSONSerializer 
{
	private static readonly fsSerializer _serializer = new fsSerializer();
	private static fsData data;

	public static void SaveToJSON<T>(T classToSave, string optionalInstanceID = "") where T : ComponentECS
	{		
		_serializer.TrySerialize(classToSave.ObjectType, classToSave, out data).AssertSuccess();

		_serializer.TrySerialize(classToSave.ObjectType, classToSave, out data).AssertSuccessWithoutWarnings();

		string filePath = Application.dataPath + "/Resources/JSON/" + classToSave.AssetParentFolder + "/" + classToSave.AssetName;
		if (!optionalInstanceID.Equals("")) filePath += "_" + optionalInstanceID;
		filePath += ".json";

		//Debug.Log("Saving to: " + filePath);
		Directory.CreateDirectory(Application.dataPath + "/Resources/JSON/" + classToSave.AssetParentFolder + "/");
		
		using (FileStream file = File.Open(filePath, FileMode.Create))
		{
			using (StreamWriter writer = new StreamWriter(file))
			{
				fsJsonPrinter.PrettyJson(data, writer);
			}
		}
#if UNITY_EDITOR
		AssetDatabase.Refresh();
#endif
	}

	public static T LoadFromJSON<T>(T classToLoad, string optionalInstanceID = "") where T : ComponentECS
	{
	    Type typeToLoad = classToLoad.GetType();
		string filePath = Application.dataPath + "/Resources/JSON/" + classToLoad.AssetParentFolder + "/" + classToLoad.AssetName;
		if (!optionalInstanceID.Equals("")) filePath += "_" + optionalInstanceID;
		filePath += ".json";

		//Debug.Log("Loading from: " + filePath);
		if (!File.Exists(filePath))
		{
			Debug.Log("No existing JSON file, creating new model now.");
			//T newT = new T(classToLoad.Owner);
		    T newT = (T) Activator.CreateInstance(classToLoad.ObjectType, classToLoad.Owner);
			SaveToJSON(newT, optionalInstanceID);
		}
		using (StreamReader reader = new StreamReader(filePath))
		{
			string strdata = reader.ReadToEnd();
			fsJsonParser.Parse(strdata, out data);
			object deserialized = null;
			_serializer.TryDeserialize(data, typeToLoad, ref deserialized).AssertSuccess();

		    if (deserialized != null)
		    {
		        return deserialized as T;
		    }
			else //make new T b/c JSON doesn't exist
			{
                Debug.Log("Not deserialized");
                return (T)Activator.CreateInstance(classToLoad.ObjectType, classToLoad.Owner);

            }
		}
		
	}

	public static void OverwriteFromJSON<T>( T classToLoad, string optionalInstanceID = "") where T : ComponentECS, new()
	{
		string filePath = Application.dataPath + "/Resources/JSON/" + classToLoad.AssetParentFolder + "/" + classToLoad.AssetName;
		if (!optionalInstanceID.Equals("")) filePath += "_" + optionalInstanceID;
		filePath += ".json";

		//Debug.Log("Overwriting from: " + filePath);
		if (!File.Exists(filePath))
		{
			Debug.Log("No existing JSON file, creating new model file before overwrite.");
			T newT = new T();
			SaveToJSON(newT, optionalInstanceID);
			//var newT = Activator.CreateInstance(classToLoadType);
			//SaveToJSON(newT, optionalInstanceID);
		}
		//using (StreamReader reader = new StreamReader(filePath))
		//{
		//	string strdata = reader.ReadToEnd();
		//	//EditorJsonUtility.FromJsonOverwrite(strdata, classToLoad);
		//}
	}

	public static void OverwriteNew<T>(T classToLoad, string optionalInstanceID = "") where T : ComponentECS, new()
	{
		string filePath = Application.dataPath + "/Resources/JSON/" + classToLoad.AssetParentFolder + "/" + classToLoad.AssetName;
		if (!optionalInstanceID.Equals("")) filePath += "_" + optionalInstanceID;
		filePath += ".json";

		//Debug.Log("Loading from: " + filePath);
		if (!File.Exists(filePath))
		{
			Debug.Log("No existing JSON file, creating new model now.");
			T newT = new T();
			SaveToJSON(newT, optionalInstanceID);
		}
		using (StreamReader reader = new StreamReader(filePath))
		{
			string strdata = reader.ReadToEnd();
			fsJsonParser.Parse(strdata, out data);

			object deserialized = null;
			_serializer.TryDeserialize(data, typeof(T), ref deserialized).AssertSuccess();

			if (deserialized != null)
			{
				T newClass = (T)deserialized;
				//classToLoad = (T) newClass.Clone();
				classToLoad = newClass;
			}
			else //make new T b/c JSON doesn't exist
			{
				classToLoad = new T();
			}
		}
	}

	/*public static void OverwriteFromJSON(Type classToLoadType, ComponentECS classToLoad, string optionalInstanceID = "") //where T : ComponentECS, new()
		{
			string filePath = Application.dataPath + "/Resources/JSON/" + classToLoad.AssetParentFolder + "/" + classToLoad.AssetName;
			if (!optionalInstanceID.Equals("")) filePath += "_" + optionalInstanceID;
			filePath += ".json";

			Debug.Log("Overwriting from: " + filePath);
			if (!File.Exists(filePath))
			{
				Debug.Log("No existing JSON file, Nothing from which to overwrite.");
				//return;
				var newT = Activator.CreateInstance(classToLoadType);
				//SaveToJSON(newT, optionalInstanceID);
			}
			using (StreamReader reader = new StreamReader(filePath))
			{
				string strdata = reader.ReadToEnd();
				JsonUtility.FromJsonOverwrite(strdata, classToLoad);
			}
		}*/

	}
