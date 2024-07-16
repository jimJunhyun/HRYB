using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Progress;



public class CollectionUI : MonoBehaviour, IOpenableWindowUI
{
	Transform content;
	public const string COLLECTIONBUTTON = "CollectionItem";
	List<CollectionButtonUI> buttons = new List<CollectionButtonUI>();

	YinyangItemDetailUI detail;

	private void Awake()
	{
		content = transform.Find("Left Section/ItemView/Viewport/Content");
		detail = GetComponent<YinyangItemDetailUI>();
		
	}

	public void OnOpen()
	{
		if(detail == null)
		{
			detail = GetComponent<YinyangItemDetailUI>();
		}

		Debug.Log("$$$$$$$$$$$$" + GameManager.instance.saver.pedia.materialCollections.Values.Count);
		bool first = true;
		foreach (ItemCollection item in GameManager.instance.saver.pedia.materialCollections.Values)
		{
			if (first)
			{
				detail.SetInfo(item);
			}
			Debug.Log(item.myItem.MyName + " : " + item.myItem.desc);
			GameObject g = PoolManager.GetObject(COLLECTIONBUTTON, content);
			Debug.Log(g == null);
			CollectionButtonUI btn = g.GetComponent<CollectionButtonUI>();
			Debug.Log(btn == null);
			btn.SetInfo(item);
			buttons.Add(btn);
		}
	}

	public void OnClose()
	{
		for (int i = 0; i < buttons.Count; i++)
		{
			PoolManager.ReturnObject(buttons[i].gameObject);
		}
		buttons.Clear();
	}

	public void WhileOpening()
	{
	}

	public void Refresh()
	{
		if (detail == null)
		{
			detail = GetComponent<YinyangItemDetailUI>();
		}
		for (int i = 0; i < buttons.Count; i++)
		{
			buttons[i].RefreshInfo();
		}
		detail.RefreshInfo();
	}
}
