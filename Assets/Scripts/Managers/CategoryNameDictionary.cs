using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CategoryImagePair
{
	public string name;
	public Sprite sprite;

	public CategoryImagePair(string n, Sprite sp)
	{
		name = n;
		sprite = sp;
	}
}

[System.Serializable]
public class SerializedNameSpritePairs : Dictionary<string, Sprite>, ISerializationCallbackReceiver
{
	[SerializeField]
	public List<CategoryImagePair> keyValues = new List<CategoryImagePair>();

	public void OnAfterDeserialize()
	{
		this.Clear();


		for (int i = 0; i < keyValues.Count; i++)
		{
			if (this.ContainsKey(keyValues[i].name))
			{
				keyValues[i].name += '0';
			}
			this.Add(keyValues[i].name, keyValues[i].sprite);
		}
	}

	public void OnBeforeSerialize()
	{
		keyValues.Clear();
		foreach (KeyValuePair<string, Sprite> pair in this)
		{
			keyValues.Add(new CategoryImagePair(pair.Key, pair.Value));
		}
	}
}

[CreateAssetMenu()]
public class CategoryImages : ScriptableObject
{
    public SerializedNameSpritePairs info = new SerializedNameSpritePairs();
}
