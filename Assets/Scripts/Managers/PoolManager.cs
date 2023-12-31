using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StackWithName<T>{
	public Stack<T> data;
	public string name;

	public StackWithName(string n)
	{
		name = n;
		data = new Stack<T>();
	}
}

public class PoolManager : MonoBehaviour
{
	PoolList list;
	static List<StackWithName<GameObject>> pooleds = new List<StackWithName<GameObject>>();

	public void Awake()
	{
		list = Resources.Load<PoolList>("PoolList");

		for (int i = 0; i < list.poolList.Count; i++)
		{
			pooleds.Add(new StackWithName<GameObject>(list.poolList[i].obj.name));
			for (int j = 0; j < list.poolList[i].num; j++)
			{
				GameObject o = Instantiate(list.poolList[i].obj, Vector3.zero, Quaternion.identity, transform);
				o.name = list.poolList[i].obj.name;
				o.SetActive(false);
				pooleds[i].data.Push(o);
			}
		}
	}
	
	public static GameObject GetObject(string name, Vector3 pos, Quaternion rot)
	{
		StackWithName<GameObject> st;
		if ((st = pooleds.Find(item => item.name == name)) != null)
		{
			if(st.data.Count > 0)
			{
				GameObject res = st.data.Pop();
				res.SetActive(true);
				res.transform.position = pos;
				res.transform.rotation = rot;
				return res;
			}
		}
		Debug.LogError($"Item named {name} doesn't exist!");
		return null;
	}
	
	public static GameObject GetObject(string name, Transform parent)
	{
		StackWithName<GameObject> st;
		if ((st = pooleds.Find(item => item.name == name)) != null)
		{
			GameObject res = st.data.Pop();
			res.SetActive(true);
			res.transform.SetParent(parent);
			res.transform.localPosition = Vector3.zero;
			res.transform.localRotation = Quaternion.identity;
			return res;
		}
		Debug.LogError($"Item named {name} doesn't exist!");
		return null;
	}

	public static GameObject GetObject(string name, Vector3 pos, Vector3 forward)
	{
		StackWithName<GameObject> st;
		if ((st = pooleds.Find(item => item.name == name)) != null)
		{
			if(st.data.Count > 0)
			{
				GameObject res = st.data.Pop();
				res.SetActive(true);
				res.transform.position = pos;
				res.transform.forward = forward;
				return res;
			}
		}
		Debug.LogError($"Item named {name} doesn't exist!");
		return null;
	}

	public static void ReturnObject(GameObject obj)
	{
		StackWithName<GameObject> st;
		if ((st = pooleds.Find(item => item.name == obj.name)) != null)
		{
			obj.SetActive(false);
			
			obj.transform.position = Vector3.zero;
			obj.transform.rotation = Quaternion.identity;
			obj.transform.localScale = Vector3.one;
			st.data.Push(obj);
		}
		else
		{
			Debug.LogError($"{obj.name} doesn't exist in pool!");
		}
	}
}
