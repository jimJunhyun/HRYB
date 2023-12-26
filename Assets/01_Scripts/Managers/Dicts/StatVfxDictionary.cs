using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// (statEffId -> (EffectPoses -> List(GameObject)) * EffectPoses.Length)
/// </summary>

public enum EffectPoses
{
	Trail,
	Whirl,
	Hit,

	Max
}

[System.Serializable]
public class IdEffPair
{
	public StatEffID id;
	public EffDict effects;

	public IdEffPair(StatEffID p, EffDict effs)
	{
		id = p;
		effects = effs;
	}
}

[System.Serializable]
public class PosEffPair
{
	public EffectPoses poses;
	public List<GameObject> effects;

	public PosEffPair(EffectPoses p, List<GameObject> effs)
	{
		poses = p;
		effects = effs;
	}
}

[System.Serializable]
public class EffDict : Dictionary<EffectPoses, List<GameObject>>, ISerializationCallbackReceiver
{
	public List<PosEffPair> posEffs;

	public void OnAfterDeserialize()
	{
		this.Clear();


		for (int i = 0; i < posEffs.Count; i++)
		{
			if(posEffs[i].poses >= EffectPoses.Hit)
				continue;
			if (this.ContainsKey(posEffs[i].poses))
			{
				posEffs[i].poses += 1;
			}
			this.Add(posEffs[i].poses, posEffs[i].effects);
		}
	}

	public void OnBeforeSerialize()
	{
		posEffs.Clear();
		foreach (KeyValuePair<EffectPoses, List<GameObject>> pair in this)
		{
			posEffs.Add(new PosEffPair(pair.Key, pair.Value));
		}
	}
}

[System.Serializable]
public class StatVfxDictionary : Dictionary<StatEffID, EffDict>
{
    public List<IdEffPair> idEff;

	public void OnAfterDeserialize()
	{
		this.Clear();


		for (int i = 0; i < idEff.Count; i++)
		{
			if (this.ContainsKey(idEff[i].id))
			{
				if(idEff[i].id < StatEffID.Max)
				{
					idEff[i].id += 1;
				}
				else
					continue;
				for (int j = ((int)EffectPoses.Max) - 1; j >= idEff[i].effects.posEffs.Count; --j)
				{
					idEff[i].effects.Add((EffectPoses)j, new List<GameObject>());
				}
			}
			this.Add(idEff[i].id, idEff[i].effects);
		}
	}

	public void OnBeforeSerialize()
	{
		idEff.Clear();
		foreach (KeyValuePair<StatEffID, EffDict> pair in this)
		{
			idEff.Add(new IdEffPair(pair.Key, pair.Value));
		}
	}
}
