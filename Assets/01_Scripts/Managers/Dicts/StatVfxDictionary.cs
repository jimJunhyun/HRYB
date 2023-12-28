using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EffectPoses
{
	Trail,
	Whirl,
	Hit,

	Max
}


[System.Serializable]
public class StatVfxDict : Dictionary<StatEffID, List<SerializePair<EffectPoses, string>>>, ISerializationCallbackReceiver
{
    public List<SerializePair<StatEffID, List<SerializePair<EffectPoses, string>>>> idEff = 
		new List<SerializePair<StatEffID, List<SerializePair<EffectPoses, string>>>>();

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
		foreach (KeyValuePair<StatEffID, List<SerializePair<EffectPoses, string>>> pair in this)
		{
			idEff.Add(new SerializePair<StatEffID, List<SerializePair<EffectPoses, string>>>(pair.Key, pair.Value));
		}
	}

	public StatVfxDict()
	{
		for (int i = 0; i < ((int)StatEffID.Max); i++)
		{
			idEff.Add(new SerializePair<StatEffID, List<SerializePair<EffectPoses, string>>>((StatEffID)i, new List<SerializePair<EffectPoses, string>>()));
		}
		OnAfterDeserialize();
	}
}

[CreateAssetMenu()]
public class StatVfxDictionary : ScriptableObject
{
	public StatVfxDict data;
}