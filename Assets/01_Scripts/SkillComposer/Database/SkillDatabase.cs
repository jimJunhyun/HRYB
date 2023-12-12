using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class NameSkillPair
{
	public string name;
	public SkillRoot skill;
	public NameSkillPair(string n, SkillRoot sk)
	{
		name = n;
		skill = sk;
	}
}

[System.Serializable]
public class SerializedNameSkillDictionary : Dictionary<string, SkillRoot>, ISerializationCallbackReceiver
{
	[SerializeField]
	public List<NameSkillPair> keyValues = new List<NameSkillPair>();

	public void OnAfterDeserialize()
	{
		this.Clear();


		for (int i = 0; i < keyValues.Count; i++)
		{
			if (this.ContainsKey(keyValues[i].name))
			{
				keyValues[i].name += '0';
			}
			this.Add(keyValues[i].name, keyValues[i].skill);
		}
	}

	public void OnBeforeSerialize()
	{
		keyValues.Clear();
		foreach (KeyValuePair<string, SkillRoot> pair in this)
		{
			keyValues.Add(new NameSkillPair(pair.Key, pair.Value));
		}
	}
}

[CreateAssetMenu(menuName = "Skills/Database")]
public class SkillDatabase : ScriptableObject
{
    public SerializedNameSkillDictionary info;
}
