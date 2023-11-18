using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EngToKor
{
	public string eng;
	public string kor;
	public EngToKor(string e, string k)
	{
		eng = e;
		kor = k;
	}
}

[System.Serializable]
public class SerializedDictionary : Dictionary<string, string>, ISerializationCallbackReceiver
{
	[SerializeField]
	public List<EngToKor> keyValues = new List<EngToKor>();

	public void OnAfterDeserialize()
	{
		this.Clear();


		for (int i = 0; i < keyValues.Count; i++)
		{
			if (this.ContainsKey(keyValues[i].eng))
			{
				keyValues[i].eng += '0';
			}
			this.Add(keyValues[i].eng, keyValues[i].kor);
		}
	}

	public void OnBeforeSerialize()
	{
		keyValues.Clear();
		foreach (KeyValuePair<string, string> pair in this)
		{
			keyValues.Add(new EngToKor(pair.Key, pair.Value));
		}
	}
}

[CreateAssetMenu()]
public class ItemNameDictionary : ScriptableObject
{

	public SerializedDictionary Dict;
}
