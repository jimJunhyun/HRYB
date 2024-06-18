using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class SkillLoader
{
	private static SkillDatabase HumanSkillDb;
	private static SkillDatabase YohoSkillDb;
	public SkillLoader()
	{
		
		AssetBundle asset = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "skilldatabase"));
		AssetBundle asset2 = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "yohoSkilldataBase"));
		SkillDatabase database = null;
		SkillDatabase database2 = null;
		if (asset != null)
		{
			database= asset.LoadAsset<SkillDatabase>("SkillDatabase");
		}
		if(asset2 != null)
		{
			database2 = asset2.LoadAsset<SkillDatabase>("YohoSkilldataBase");
		}

		if (database != null)
		{
			HumanSkillDb = database;
			Debug.Log("Skill Database Load Successed.");
		}
		else
		{
			Debug.LogError("Skill Database Load Failed.");
		}

		if (database2 != null)
		{
			YohoSkillDb = database2;
			Debug.Log("Skill Database2 Load Successed.");
		}
		else
		{
			Debug.LogError("Skill Database2 Load Failed.");
		}
		if (asset == null || asset2 == null)
			Debug.LogError($"LOAD FAIL : {asset == null}/{asset2 == null}");

		
	}

	//public SkillRoot this[string name]
	//{
	//	get=>skillDb.info[name];
	//}

	public SkillRoot GetHumanSkill(string name)
	{
		return HumanSkillDb.info[name];
	}

	public SkillRoot GetYohoSkill(String name)
	{
		return YohoSkillDb.info[name];
	}

	public SkillRoot GetSkill(string name)
	{
		if (HumanSkillDb.info.ContainsKey(name))
		{
			return HumanSkillDb.info[name];

		}
		else if (YohoSkillDb.info.ContainsKey(name))
		{
			return YohoSkillDb.info[name];
		}
		Debug.LogError($"그런 이름의 스킬은 없습니다 : {name}");
		return null;
	}
}
