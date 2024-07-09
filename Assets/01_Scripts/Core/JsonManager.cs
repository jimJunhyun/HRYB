using UnityEngine;
using System.IO;

public static class JsonManager<T>
{
    public static bool LoadJson(string name, out T output)
	{
		string path = Path.Combine(Application.dataPath, name);
		if(File.Exists(path))
		{
			string jsonData = File.ReadAllText(path);
			output = JsonUtility.FromJson<T>(jsonData);
			if (output == null) return false;
			return true;
		}
		output = default;
		return false;
	}
	
	public static bool SaveJson(T item, string name)
	{
		string jsonData = JsonUtility.ToJson(item);
		string path = Path.Combine(Application.dataPath, name);
		File.WriteAllText(path, jsonData);
		return true;
	}
}
