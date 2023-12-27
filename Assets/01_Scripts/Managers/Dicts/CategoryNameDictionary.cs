using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializePair<T1, T2>
{
	public T1 key;
	public T2 value;

	public SerializePair(T1 k, T2 v)
	{
		key = k;
		value = v;
	}
}

[System.Serializable]
public class SerializedNameSpritePairs : Dictionary<string, Sprite>, ISerializationCallbackReceiver
{
	[SerializeField]
	public List<SerializePair<string, Sprite>> keyValues = new List<SerializePair<string, Sprite>>();

	public void OnAfterDeserialize()
	{
		this.Clear();


		for (int i = 0; i < keyValues.Count; i++)
		{
			if (this.ContainsKey(keyValues[i].key))
			{
				keyValues[i].key += '0';
			}
			this.Add(keyValues[i].key, keyValues[i].value);
		}
	}

	public void OnBeforeSerialize()
	{
		keyValues.Clear();
		foreach (KeyValuePair<string, Sprite> pair in this)
		{
			keyValues.Add(new SerializePair<string, Sprite>(pair.Key, pair.Value));
		}
	}
}

[CreateAssetMenu()]
public class CategoryNameDictionary : ScriptableObject
{
    public SerializedNameSpritePairs info;
}
