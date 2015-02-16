using UnityEngine;
using UnityEditor;
using System;

// sets FBX Mesh Scale Factor to 1
public class FBX_Import : AssetPostprocessor 
{
	public const float importScale = 1.0F;

	void OnPreprocessModel()
	{
		if(assetPath.ToLower().Contains(".fbx") )
		{
			Debug.Log("Asset Path: " + assetPath);
			ModelImporter importer = assetImporter as ModelImporter;
			importer.globalScale = importScale;
			importer.animationType = ModelImporterAnimationType.None;
		}
	}

	void OnPostProcessGameObjectWIthUserProperties(GameObject go, String[] propNames, System.Object[] values)
	{
		for (int i = 0; i < propNames.Length; i++) {
			
			Debug.Log("Propname: " + propNames[i] + " value: " +values[i]);
		}
	}
	
}
