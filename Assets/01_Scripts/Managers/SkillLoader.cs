using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class SkillLoader
{
	private SkillDatabase HumenSkillDb;
	private SkillDatabase YohoSkillDb;
	public SkillLoader()
	{
		var asset = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "skilldatabase"));
		var asset2 = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "yohoSkilldataBase"));
		if (asset == null && asset2 == null)
			Debug.LogError("LOAD FAIL");

		SkillDatabase database = asset.LoadAsset<SkillDatabase>("SkillDatabase");
		SkillDatabase database2 = asset2.LoadAsset<SkillDatabase>("YohoSkilldataBase");
		if(database != null)
		{
			HumenSkillDb = database;
			Debug.Log("Skill Database Load Successed.");
		}
		else
		{
			Debug.LogError("Skill Database Load Failed.");
		}

		if (database2 != null)
		{
			YohoSkillDb = database2;
			Debug.Log("Skill Database Load Successed.");
		}
		else
		{
			Debug.LogError("Skill Database Load Failed.");
		}
	}

	//public SkillRoot this[string name]
	//{
	//	get=>skillDb.info[name];
	//}

	public SkillRoot GetHumenSkill(string name)
	{
		return HumenSkillDb.info[name];
	}

	public SkillRoot GetYohoSkill(String name)
	{
		return YohoSkillDb.info[name];
	}
}
