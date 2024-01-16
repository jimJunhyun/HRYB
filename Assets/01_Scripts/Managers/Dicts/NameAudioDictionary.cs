using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class NameAudioDict : Dictionary<string, AudioClip>, ISerializationCallbackReceiver
{
	[SerializeField]
	public List<SerializePair<string, AudioClip>> keyValues = new List<SerializePair<string, AudioClip>>();

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
		foreach (KeyValuePair<string, AudioClip> pair in this)
		{
			keyValues.Add(new SerializePair<string, AudioClip>(pair.Key, pair.Value));
		}
	}
}

[CreateAssetMenu()]
public class NameAudioDictionary : ScriptableObject
{
	public NameAudioDict data;
}