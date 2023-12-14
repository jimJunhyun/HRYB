using GSpawn;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EffectEnum
{
	
}

public class EffectManager : MonoBehaviour
{
	PoolList list;
	static List<StackWithName<EffectObject>> pooleds = new List<StackWithName<EffectObject>>();
	public void Awake()
	{
		list = Resources.Load<PoolList>("EffectList");

		for (int i = 0; i < list.poolList.Count; i++)
		{
			pooleds.Add(new StackWithName<EffectObject>(list.poolList[i].obj.name));
			for (int j = 0; j < list.poolList[i].num; j++)
			{
				GameObject o = Instantiate(list.poolList[i].obj, Vector3.zero, Quaternion.identity, transform);
				o.name = list.poolList[i].obj.name;
				o.SetActive(false);
				try
				{
					pooleds[i].data.Push(o.GetComponent<EffectObject>());
				}
				catch
				{
					Debug.LogError($"{o} : 해당 오브잭트는 EffectPool이 없음");
				}
			}
		}
	}
	
	public static EffectObject GetObject(string name, Vector3 pos, Quaternion rot)
	{
		StackWithName<EffectObject> st;
		if ((st = pooleds.Find(item => item.name == name)) != null)
		{
			if(st.data.Count > 0)
			{
				EffectObject res = st.data.Pop();
				res.gameObject.SetActive(true);
				res.transform.position = pos;
				res.transform.rotation = rot;
				res.transform.parent = null;
				return res;
			}
		}
		Debug.LogError($"Item named {name} doesn't exist!");
		return null;
	}
	
	public static EffectObject GetObject(string name, Transform parent)
	{
		StackWithName<EffectObject> st;
		if ((st = pooleds.Find(item => item.name == name)) != null)
		{
			EffectObject res = st.data.Pop();
			res.gameObject.SetActive(true);
			res.transform.SetParent(parent);
			res.transform.localPosition = res._originPosision;
			res.transform.localEulerAngles = parent.localEulerAngles +  res._originQuaternion;
			res.transform.parent = null;
			return res;
		}
		Debug.LogError($"Item named {name} doesn't exist!");
		return null;
	}
	
	public static void ReturnObject(EffectObject obj)
	{
		StackWithName<EffectObject> st;
		if ((st = pooleds.Find(item => item.name == obj.name)) != null)
		{
			obj.gameObject.SetActive(false);
			
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
