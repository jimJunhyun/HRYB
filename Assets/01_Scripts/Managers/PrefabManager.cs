using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
	public GameObject invenSlot;
	public GameObject quickSlot;
	public GameObject bossHPBar;
	public void Awake()
	{
		var inven = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "inven"));
		if (inven == null)
			Debug.LogError("LOAD FAIL");
		
		 invenSlot = inven.LoadAsset<GameObject>("Inven");

		var quick = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "quickslot"));
		if (quick == null)
			Debug.LogError("LOAD FAIL");

		quickSlot = quick.LoadAsset<GameObject>("QuickSlot");

		var bossHP = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "bosshp"));
		if (bossHP == null)
			Debug.LogError("LOAD FAIL");

		bossHPBar = bossHP.LoadAsset<GameObject>("BossHP");
	}
}
