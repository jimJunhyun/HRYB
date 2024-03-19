using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

// #if UNITY_EDITOR
// [InitializeOnLoad]
// #endif
public class AssetBundleBuilder
{
	[MenuItem("Assets/Build AssetBundles")]
	static void BuildAllAssetBundles()
	{
		string assetBundleDirectory = "Assets/StreamingAssets";
		if (!Directory.Exists(assetBundleDirectory))
		{
			Directory.CreateDirectory(assetBundleDirectory);
		}
		BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
		Debug.Log("BUILDED");
	}

	static AssetBundleBuilder()
	{
		Debug.Log("BUILDING");
		BuildAllAssetBundles();
	}
}
