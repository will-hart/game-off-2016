using System;
using System.IO;
using UnityEngine;
using UnityEditor;

public class FBXScaleOverride : AssetPostprocessor
{
	private string name;

	void OnPreprocessModel()
	{
		ModelImporter importer = assetImporter as ModelImporter;
		name = importer.assetPath.ToLower();

		if (name.Substring(name.Length - 4, 4) == ".fbx")
		{
			importer.materialName = ModelImporterMaterialName.BasedOnMaterialName;
			importer.materialSearch = ModelImporterMaterialSearch.Everywhere;
			if (!assetPath.Contains("_ANIM"))
				importer.animationType = ModelImporterAnimationType.Legacy;
		}
	}

	void OnPostprocessModel(GameObject g)
	{
		g.transform.localScale = Vector3.one;

		//var modelName = Path.GetFileName(assetPath.Substring(0, assetPath.LastIndexOf(".fbx", StringComparison.Ordinal)));
		//string asspath = assetPath.Replace(modelName + ".fbx", "");
		//var fbmPath = assetPath.Replace(".fbx", ".fbm");
		////var matPath = assetPath.Replace(".fbx", "/Materials/");
		////var matPath = asspath + "Materials/";
		//
		//DirectoryInfo dir = new DirectoryInfo(fbmPath);
		//FileInfo[] files = dir.GetFiles();
		//foreach (FileInfo file in files)
		//{
		//	var pardir = dir.Parent;
		//	//pardir = System.IO.Directory.GetParent(pardir.)
		//	var thematdir = System.IO.Path.Combine(pardir.FullName, "Materials");
		//	var fullmatfile = System.IO.Path.Combine(thematdir, file.Name);
		//	//Debug.Log($"Moving from: {file.FullName} to: {fullmatfile}");
		//	try
		//	{
		//		File.Move(file.FullName, fullmatfile);
		//	}
		//	catch (IOException e)
		//	{
		//		D.Log(e);
		//	}
		//	//string status = AssetDatabase.MoveAsset(
		//	//			 fbmPath + file.Name,
		//	//			 matPath + file.Name);
		//
		//	// Everything is OK if status is empty
		//	//if (status != "")
		//	//{
		//	//	Debug.Log(status);
		//	//}
		//
		//}
		////Debug.Log($"Deleting {dir.FullName}");
		//dir.Delete(true);
		//AssetDatabase.Refresh();
		//var tex = (Texture2D)(AssetDatabase.LoadAssetAtPath(fbmPath + "/" + modelName + "-0_vtk_texture.png", typeof(Texture2D)));
		//matProps.Texture = tex;
	}
}