using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Progress;



public class CollectionUI : MonoBehaviour, IOpenableWindowUI
{
	Transform content;
	public const string COLLECTIONBUTTON = "CollectionItem";
	List<GameObject> buttons = new List<GameObject>();

	YinyangItemDetailUI detail;

	private void Awake()
	{
		content = transform.Find("ItemView/Viewport/Content");
		detail = GetComponent<YinyangItemDetailUI>();
	}

	public void OnOpen()
	{
		Debug.Log(GameManager.instance.pedia.materialCollections.Values.Count);
		bool first = true;
		foreach (ItemCollection item in GameManager.instance.pedia.materialCollections.Values)
		{
			if (first)
			{
				detail.SetInfo(item);
			}
			Debug.Log(item.myItem.MyName + " : " + item.myItem.desc);
			GameObject g = PoolManager.GetObject(COLLECTIONBUTTON, content);
			CollectionButtonUI btn = g.GetComponent<CollectionButtonUI>();
			btn.SetInfo(item);
			buttons.Add(g);
		}
	}

	public void OnClose()
	{
		for (int i = 0; i < buttons.Count; i++)
		{
			PoolManager.ReturnObject(buttons[i]);
		}
		buttons.Clear();
	}

	public void WhileOpening()
	{
	}

	public void Refresh()
	{
		for (int i = 0; i < buttons.Count; i++)
		{
			if(buttons[i].GetComponent<CollectionButtonUI>() != null)
			{
				buttons[i].GetComponent<CollectionButtonUI>().RefreshInfo();
			}


			if(buttons[i].GetComponent<YinyangItemDetailUI>() != null)
			{
				buttons[i].GetComponent<YinyangItemDetailUI>().RefreshInfo();
			}

			
			
		}
	}
}
