using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// (statEffId -> List(EffPos : Eff))
/// </summary>

public enum EffectPoses
{
	Trail,
	Whirl,
	Hit,

	Max
}


[System.Serializable]
public class StatVfxDict : Dictionary<StatEffID, List<SerializePair<EffectPoses, string>>>
{
    public List<SerializePair<StatEffID, List<SerializePair<EffectPoses, string>>>> idEff;

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
			idEff.Add(new SerializePair<StatEffID, List<SerializePair<EffectPoses, string>>> (pair.Key, pair.Value));
		}
	}
}

[CreateAssetMenu()]
public class StatVfxDictionary : ScriptableObject
{
	public StatVfxDict data;
}