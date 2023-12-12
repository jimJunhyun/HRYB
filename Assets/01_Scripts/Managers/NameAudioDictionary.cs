using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NameAudioPair
{
	public string name;
	public AudioClip sound;

	public NameAudioPair(string n, AudioClip s)
	{
		name = n;
		sound = s;
	}
}

[System.Serializable]
public class NameAudioDictionary : Dictionary<string, AudioClip>, ISerializationCallbackReceiver
{
	[SerializeField]
	public List<NameAudioPair> keyValues = new List<NameAudioPair>();

	public void OnAfterDeserialize()
	{
		this.Clear();


		for (int i = 0; i < keyValues.Count; i++)
		{
			if (this.ContainsKey(keyValues[i].name))
			{
				keyValues[i].name += '0';
			}
			this.Add(keyValues[i].name, keyValues[i].sound);
		}
	}

	public void OnBeforeSerialize()
	{
		keyValues.Clear();
		foreach (KeyValuePair<string, AudioClip> pair in this)
		{
			keyValues.Add(new NameAudioPair(pair.Key, pair.Value));
		}
	}
}
