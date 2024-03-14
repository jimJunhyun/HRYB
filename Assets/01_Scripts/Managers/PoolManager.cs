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
	static List<StackWithName<GameObject>> pooleds = new List<StackWithName<GameObject>>();

	static StringBuilder sb = new StringBuilder();
	static Transform self;

	public void Awake()
	{
		self = transform;

		List<PoolElement> datas = new List<PoolElement>(Resources.LoadAll<PoolElement>("PoolList"));

		for (int i = 0; i < datas.Count; i++)
		{
			pooleds.Add(new StackWithName<GameObject>(datas[i].data.obj.name));
			for (int j = datas[i].data.num - 1; j >= 0; --j)
			{
				GameObject o = Instantiate(datas[i].data.obj, Vector3.zero, Quaternion.identity, transform);
				sb.Append(datas[i].data.obj.name);
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
		sb.Clear();
	}
	
	public static GameObject GetObject(string name, Vector3 pos, Quaternion rot)
	{
		StackWithName<GameObject> st;
		int idx = pooleds.FindIndex(item => SimilarName(item.name, name));
		if (idx != -1 && (st = pooleds[idx]) != null)
		{
			if(st.data.Count > 1)
			{
				GameObject res = st.data.Pop();
				res.SetActive(true);
				res.transform.position = pos;
				res.transform.rotation = rot;
				pooleds[idx] = st;
				return res;
			}
			else
			{

				GameObject res = st.data.Peek();

				GameObject added = Instantiate(res, Vector3.zero, Quaternion.identity, self);
				string[] str = res.name.Trim('&').Split('&');
				sb.Append(str[0]);
				sb.Append('&');
				sb.Append(int.Parse(str[1]) + 1);
				sb.Append('&');
				added.name = sb.ToString();
				sb.Clear();
				added.SetActive(false);

				res.SetActive(true);
				res.transform.position = pos;
				res.transform.rotation = rot;
				st.data.Pop();

				st.data.Push(added);
				pooleds[idx] = st;
				return res;
			}
		}
		Debug.LogWarning($"Item named {name} doesn't exist!");
		return null;
	}

	public static GameObject GetObject(string name, Vector3 pos, Quaternion rot, float lifetime)
	{
		StackWithName<GameObject> st;
		int idx = pooleds.FindIndex(item => SimilarName(item.name, name));
		if (idx != -1 && (st = pooleds[idx]) != null)
		{
			if (st.data.Count > 1)
			{
				GameObject res = st.data.Pop();
				res.SetActive(true);
				res.transform.position = pos;
				res.transform.rotation = rot;
				GameManager.instance.StartCoroutine(DelReturner(res, lifetime));
				pooleds[idx] = st;
				return res;
			}
			else
			{
				GameObject res = st.data.Peek();

				GameObject added = Instantiate(res, Vector3.zero, Quaternion.identity, self);
				string[] str = res.name.Trim('&').Split('&');
				sb.Append(str[0]);
				sb.Append('&');
				sb.Append(int.Parse(str[1]) + 1);
				sb.Append('&');
				added.name = sb.ToString();
				sb.Clear();
				added.SetActive(false);

				res.SetActive(true);
				res.transform.position = pos;
				res.transform.rotation = rot;
				GameManager.instance.StartCoroutine(DelReturner(res, lifetime));
				st.data.Pop();

				st.data.Push(added);
				pooleds[idx] = st;
				return res;
			}
		}
		Debug.LogWarning($"Item named {name} doesn't exist!");
		return null;
	}

	public static GameObject GetObject(string name, Transform parent)
	{
		StackWithName<GameObject> st;
		int idx = pooleds.FindIndex(item => SimilarName(item.name, name));
		if (idx != -1 && (st = pooleds[idx]) != null)
		{
			if(st.data.Count > 1)
			{
				GameObject res = st.data.Pop();
				res.SetActive(true);
				res.transform.SetParent(parent);
				res.transform.localPosition = Vector3.zero;
				res.transform.localRotation = Quaternion.identity;
				pooleds[idx] = st;
				return res;
			}
			else
			{
				GameObject res = st.data.Peek();

				GameObject added = Instantiate(res, Vector3.zero, Quaternion.identity, self);
				string[] str = res.name.Trim('&').Split('&');
				sb.Append(str[0]);
				sb.Append('&');
				sb.Append(int.Parse(str[1]) + 1);
				sb.Append('&');
				added.name = sb.ToString();
				sb.Clear();
				added.SetActive(false);

				res.SetActive(true);
				res.transform.SetParent(parent);
				res.transform.localPosition = Vector3.zero;
				res.transform.localRotation = Quaternion.identity;
				st.data.Pop();

				st.data.Push(added);
				pooleds[idx] = st;
				return res;
			}

		}
		Debug.LogWarning($"Item named {name} doesn't exist!");
		return null;
	}

	public static GameObject GetObject(string name, Transform parent, float lifetime)
	{
		StackWithName<GameObject> st;
		int idx = pooleds.FindIndex(item => SimilarName(item.name, name));
		if (idx != -1 && (st = pooleds[idx]) != null)
		{
			if(st.data.Count > 1)
			{
				GameObject res = st.data.Pop();
				res.SetActive(true);
				res.transform.SetParent(parent);
				res.transform.localPosition = Vector3.zero;
				res.transform.localRotation = Quaternion.identity;
				GameManager.instance.StartCoroutine(DelReturner(res, lifetime));
				pooleds[idx] = st;
				return res;
			}
			else
			{
				GameObject res = st.data.Peek();

				GameObject added = Instantiate(res, Vector3.zero, Quaternion.identity, self);
				string[] str = res.name.Trim('&').Split('&');
				sb.Append(str[0]);
				sb.Append('&');
				sb.Append(int.Parse(str[1]) + 1);
				sb.Append('&');
				added.name = sb.ToString();
				sb.Clear();
				added.SetActive(false);

				res.SetActive(true);
				res.transform.SetParent(parent);
				res.transform.localPosition = Vector3.zero;
				res.transform.localRotation = Quaternion.identity;
				GameManager.instance.StartCoroutine(DelReturner(res, lifetime));
				st.data.Pop();

				st.data.Push(added);
				pooleds[idx] = st;
				return res;
			}
			
		}
		Debug.LogWarning($"Item named {name} doesn't exist!");
		return null;
	}

	public static GameObject GetObject(string name, Vector3 pos, Vector3 forward)
	{
		StackWithName<GameObject> st;
		int idx = pooleds.FindIndex(item => SimilarName(item.name, name));
		if (idx != -1 && (st = pooleds[idx]) != null)
		{

			if(st.data.Count > 1)
			{
				GameObject res = st.data.Pop();
				res.SetActive(true);
				res.transform.position = pos;
				res.transform.forward = forward;
				pooleds[idx] = st;
				return res;
			}
			else
			{
				GameObject res = st.data.Peek();

				GameObject added = Instantiate(res, Vector3.zero, Quaternion.identity, self);
				string[] str = res.name.Trim('&').Split('&');
				sb.Append(str[0]);
				sb.Append('&');
				sb.Append(int.Parse(str[1]) + 1);
				sb.Append('&');
				added.name = sb.ToString();
				sb.Clear();
				added.SetActive(false);

				res.SetActive(true);
				res.transform.position = pos;
				res.transform.forward = forward;
				st.data.Pop();

				st.data.Push(added);
				pooleds[idx] = st;
				return res;
			}
		}
		Debug.LogWarning($"Item named {name} doesn't exist!");
		return null;
	}

	public static GameObject GetObject(string name, Vector3 pos, Vector3 forward, float lifetime)
	{
		StackWithName<GameObject> st;
		int idx = pooleds.FindIndex(item => SimilarName(item.name, name));
		if (idx != -1 && (st = pooleds[idx]) != null)
		{
			if (st.data.Count > 0)
			{
				GameObject res = st.data.Pop();
				res.SetActive(true);
				res.transform.position = pos;
				res.transform.forward = forward;
				GameManager.instance.StartCoroutine(DelReturner(res, lifetime));
				pooleds[idx] = st;
				return res;
			}
			else
			{
				GameObject res = st.data.Peek();

				GameObject added = Instantiate(res, Vector3.zero, Quaternion.identity, self);
				string[] str = res.name.Trim('&').Split('&');
				sb.Append(str[0]);
				sb.Append('&');
				sb.Append(int.Parse(str[1]) + 1);
				sb.Append('&');
				added.name = sb.ToString();
				sb.Clear();
				added.SetActive(false);

				res.SetActive(true);
				res.transform.position = pos;
				res.transform.forward = forward;
				GameManager.instance.StartCoroutine(DelReturner(res, lifetime));
				st.data.Pop();

				st.data.Push(added);
				pooleds[idx] = st;
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
			
			Debug.Log("RETURNED : " + obj.name);
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
