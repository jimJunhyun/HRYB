using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class SkillLoader
{
	public SkillDatabase skillDb;
	public SkillLoader()
	{
		var asset = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "skilldatabase"));
		if (asset == null)
			Debug.LogError("LOAD FAIL");

		SkillDatabase database = asset.LoadAsset<SkillDatabase>("SkillDatabase");
		if(database != null)
		{
			skillDb = database;
			Debug.Log("Skill Database Load Successed.");
		}
		else
		{
			Debug.LogError("Skill Database Load Failed.");
		}
	}
}
