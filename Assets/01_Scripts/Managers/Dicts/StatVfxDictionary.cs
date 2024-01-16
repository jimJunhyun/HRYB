using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EffectPoses
{
	Trail,
	Whirl,
	Hit,

	Applied,

	Max
}

[System.Serializable]
public class EffPosStrPair
{
	public EffectPoses effPos;
	public string effPrefName;
	public EffPosStrPair(string s, EffectPoses p)
	{
		effPrefName = s ;
		effPos = p;
	}
}


[System.Serializable]
public class StatVfxDict : Dictionary<StatEffID, List<EffPosStrPair>>, ISerializationCallbackReceiver
{
    public List<SerializePair<StatEffID, List<EffPosStrPair>>> idEff = 
		new List<SerializePair<StatEffID, List<EffPosStrPair>>>();

	public void OnAfterDeserialize()
	{
		this.Clear();

		for (int i = 0; i < idEff.Count; i++)
		{
			
			this.Add(idEff[i].key, idEff[i].value);
		}
	}

	public void OnBeforeSerialize()
	{
		idEff.Clear();
		foreach (KeyValuePair<StatEffID, List<EffPosStrPair>> pair in this)
		{
			idEff.Add(new SerializePair<StatEffID, List<EffPosStrPair>>(pair.Key, pair.Value));
		}
	}

	public StatVfxDict()
	{
		for (int i = 0; i < ((int)StatEffID.Max); i++)
		{
			idEff.Add(new SerializePair<StatEffID, List<EffPosStrPair>>((StatEffID)i, new List<EffPosStrPair>()));
		}
		OnAfterDeserialize();
	}
}

[CreateAssetMenu()]
public class StatVfxDictionary : ScriptableObject
{
	public StatVfxDict data;
}