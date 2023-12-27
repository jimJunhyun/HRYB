using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SerializedDictionary : Dictionary<string, string>, ISerializationCallbackReceiver
{
	[SerializeField]
	public List<SerializePair<string, string>> keyValues = new List<SerializePair<string, string>>();

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
		foreach (KeyValuePair<string, string> pair in this)
		{
			keyValues.Add(new SerializePair<string, string>(pair.Key, pair.Value));
		}
	}
}

[CreateAssetMenu()]
public class ItemNameDictionary : ScriptableObject
{

	public SerializedDictionary Dict;
}
