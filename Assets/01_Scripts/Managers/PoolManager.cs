using System.Collections;
using System.Collections.Generic;
using System.Text;
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

	static Transform self;

	public void Awake()
	{
		StringBuilder sb = new StringBuilder();
		self = transform;

		list = Resources.Load<PoolList>("PoolList");

		for (int i = 0; i < list.poolList.Count; i++)
		{
			pooleds.Add(new StackWithName<GameObject>(list.poolList[i].obj.name));
			for (int j = 0; j < list.poolList[i].num; j++)
			{
				GameObject o = Instantiate(list.poolList[i].obj, Vector3.zero, Quaternion.identity, transform);
				sb.Append(list.poolList[i].obj.name);
				sb.Append('&');
				sb.Append(j);
				sb.Append('&');
				o.name = sb.ToString();
				sb.Clear();
				o.transform.GetHashCode();
				//int iId = o.transform.GetInstanceID();
				for (int k = 0; k < o.transform.childCount; k++)
				{
					Transform child = o.transform.GetChild(k);
					if(child.name == "TrailPos" || child.name == "WhirlPos")
					{
						sb.Append(o.transform.name);
						sb.Append('_');
						sb.Append(child.name);
						child.name = sb.ToString();
						sb.Clear();
					}
				}
				o.SetActive(false);
				pooleds[i].data.Push(o);
			}
		}
	}
	
	public static GameObject GetObject(string name, Vector3 pos, Quaternion rot)
	{
		StackWithName<GameObject> st;
		if ((st = pooleds.Find(item => SimilarName(item.name, name))) != null)
		{
			if(st.data.Count > 1)
			{
				GameObject res = st.data.Pop();
				res.SetActive(true);
				res.transform.position = pos;
				res.transform.rotation = rot;
				return res;
			}
			else
			{
				GameObject res = Instantiate(st.data.Peek(), Vector3.zero, Quaternion.identity, self);
				res.name = name;
				res.transform.position = pos;
				res.transform.rotation = rot;
				return res;
			}
		}
		Debug.LogWarning($"Item named {name} doesn't exist!");
		return null;
	}

	public static GameObject GetObject(string name, Vector3 pos, Quaternion rot, float lifetime)
	{
		StackWithName<GameObject> st;
		if ((st = pooleds.Find(item => SimilarName(item.name, name))) != null)
		{
			if (st.data.Count > 1)
			{
				GameObject res = st.data.Pop();
				res.SetActive(true);
				res.transform.position = pos;
				res.transform.rotation = rot;
				GameManager.instance.StartCoroutine(DelReturner(res, lifetime));
				return res;
			}
			else
			{
				GameObject res = Instantiate(st.data.Peek(), Vector3.zero, Quaternion.identity, self);
				res.name = name;
				res.transform.position = pos;
				res.transform.rotation = rot;
				GameManager.instance.StartCoroutine(DelReturner(res, lifetime));
				return res;
			}
		}
		Debug.LogWarning($"Item named {name} doesn't exist!");
		return null;
	}

	public static GameObject GetObject(string name, Transform parent)
	{
		StackWithName<GameObject> st;
		if ((st = pooleds.Find(item => SimilarName(item.name, name))) != null)
		{
			if(st.data.Count > 1)
			{
				GameObject res = st.data.Pop();
				res.SetActive(true);
				res.transform.SetParent(parent);
				res.transform.localPosition = Vector3.zero;
				res.transform.localRotation = Quaternion.identity;
				return res;
			}
			else
			{
				GameObject res = Instantiate(st.data.Peek(), Vector3.zero, Quaternion.identity, parent);
				res.name = name;
				res.transform.localPosition = Vector3.zero;
				res.transform.localRotation = Quaternion.identity;
				return res;
			}

		}
		Debug.LogWarning($"Item named {name} doesn't exist!");
		return null;
	}

	public static GameObject GetObject(string name, Transform parent, float lifetime)
	{
		StackWithName<GameObject> st;
		if ((st = pooleds.Find(item => SimilarName(item.name, name))) != null)
		{
			if(st.data.Count > 1)
			{
				GameObject res = st.data.Pop();
				res.SetActive(true);
				res.transform.SetParent(parent);
				res.transform.localPosition = Vector3.zero;
				res.transform.localRotation = Quaternion.identity;
				GameManager.instance.StartCoroutine(DelReturner(res, lifetime));
				return res;
			}
			else
			{
				GameObject res = Instantiate(st.data.Peek(), Vector3.zero, Quaternion.identity, parent);
				res.name = name;
				res.transform.localPosition = Vector3.zero;
				res.transform.localRotation = Quaternion.identity;
				return res;
			}
			
		}
		Debug.LogWarning($"Item named {name} doesn't exist!");
		return null;
	}

	public static GameObject GetObject(string name, Vector3 pos, Vector3 forward)
	{
		StackWithName<GameObject> st;
		if ((st = pooleds.Find(item => SimilarName(item.name, name))) != null)
		{
			if(st.data.Count > 1)
			{
				GameObject res = st.data.Pop();
				res.SetActive(true);
				res.transform.position = pos;
				res.transform.forward = forward;
				return res;
			}
			else
			{
				GameObject res = Instantiate(st.data.Peek(), Vector3.zero, Quaternion.identity, self);
				res.name = name;
				res.transform.position = pos;
				res.transform.forward = forward;
				return res;
			}
		}
		Debug.LogWarning($"Item named {name} doesn't exist!");
		return null;
	}

	public static GameObject GetObject(string name, Vector3 pos, Vector3 forward, float lifetime)
	{
		StackWithName<GameObject> st;
		if ((st = pooleds.Find(item => SimilarName(item.name, name))) != null)
		{
			if (st.data.Count > 0)
			{
				GameObject res = st.data.Pop();
				res.SetActive(true);
				res.transform.position = pos;
				res.transform.forward = forward;
				GameManager.instance.StartCoroutine(DelReturner(res, lifetime));
				return res;
			}
			else
			{
				GameObject res = Instantiate(st.data.Peek(), Vector3.zero, Quaternion.identity, self);
				res.name = name;
				res.transform.position = pos;
				res.transform.forward = forward;
				GameManager.instance.StartCoroutine(DelReturner(res, lifetime));
				return res;
			}
		}
		Debug.LogWarning($"Item named {name} doesn't exist!");
		return null;
	}
	
	public static void ReturnObject(GameObject obj)
	{
		StackWithName<GameObject> st;
		
		if ((st = pooleds.Find(item => SimilarName(item.name, obj.name))) != null)
		{
			obj.SetActive(false);
			obj.transform.SetParent(self);
			obj.transform.position = Vector3.zero;
			obj.transform.rotation = Quaternion.identity;
			obj.transform.localScale = Vector3.one;
			
			st.data.Push(obj);
		}
		else
		{
			Debug.LogWarning($"{obj.name} doesn't exist in pool!");
		}
	}

	public static void ReturnAllChilds(GameObject obj)
	{
		ReturnChild(obj);
	}

	static void ReturnChild(GameObject obj)
	{
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			ReturnChild(obj.transform.GetChild(i).gameObject);
		}
		ReturnObject(obj);
	}

	static IEnumerator DelReturner(GameObject obj, float t)
	{
		yield return new WaitForSeconds(t);
		ReturnObject(obj);
	}

	static bool SimilarName(string a, string b)
	{
		if(a.Trim('&').Length == a.Length && b.Trim('&').Length == b.Length)
		{
			return a == b;
		}
		else if(a.Trim('&').Length == a.Length)
		{
			string comp = b.Split('&')[0];
			return a.Equals(comp);
		}
		else if(b.Trim('&').Length == b.Length)
		{
			string comp = a.Split('&')[0];
			return b.Equals(comp);
		}
		return a == b;
		
	}
}
