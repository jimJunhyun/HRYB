using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using static UnityEditor.Progress;

public enum CollectionMode
{
	Material,
	Medicine,
}

public class CollectionUI : MonoBehaviour, IOpenableWindowUI
{
	Transform content;

	Image matButton;
	Image medButton;

	public const string COLLECTIONBUTTON = "CollectionItem";
	List<CollectionButtonUI> buttons = new List<CollectionButtonUI>();

	YinyangItemDetailUI detail;

	CollectionMode mode;

	public bool isOverlay { get; set; } = false;

	private void Awake()
	{
		content = transform.Find("Left Section/ItemView/Viewport/Content");
		matButton = transform.Find("Left Section/ItemView/ScrollViewTop/Material").GetComponent<Image>();
		medButton = transform.Find("Left Section/ItemView/ScrollViewTop/Medicine").GetComponent<Image>();
		detail = GetComponent<YinyangItemDetailUI>();
	}

	public void OnOpen()
	{
		if(detail == null)
		{
			detail = GetComponent<YinyangItemDetailUI>();
		}

		Debug.Log("$$$$$$$$$$$$" + GameManager.instance.saver.pedia.materialCollections.Values.Count);
		mode = CollectionMode.Material;
		SetMode(mode);
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

	public void SetMaterial()
	{
		if (mode == CollectionMode.Material)
			return;
		SetMode(CollectionMode.Material);
		
	}

	public void SetMedicine()
	{
		if (mode == CollectionMode.Medicine)
			return;
		SetMode(CollectionMode.Medicine);
		
	}

	public void SetMode(CollectionMode mode)
	{
		


		this.mode = mode;

		for (int i = 0; i < buttons.Count; i++)
		{
			PoolManager.ReturnObject(buttons[i].gameObject);
		}
		buttons.Clear();

		bool first = true;

		if(this.mode == CollectionMode.Material)
		{
			foreach (ItemCollection item in GameManager.instance.saver.pedia.materialCollections.Values)
			{
				if (first)
				{
					detail.SetInfo(item);
					first = false;
				}
				//Debug.Log(item.myItem.MyName + " : " + item.myItem.desc);
				GameObject g = PoolManager.GetObject(COLLECTIONBUTTON, content);
				//Debug.Log(g == null);
				CollectionButtonUI btn = g.GetComponent<CollectionButtonUI>();
				//Debug.Log(btn == null);
				btn.SetInfo(item);
				buttons.Add(btn);
			}
			matButton.color = Color.gray;
			medButton.color = Color.white;
		}
		else if(this.mode == CollectionMode.Medicine)
		{
			foreach (ItemCollection item in GameManager.instance.saver.pedia.medicineCollections.Values)
			{
				if (first)
				{
					detail.SetInfo(item);
					first = false;
				}
				GameObject g = PoolManager.GetObject(COLLECTIONBUTTON, content);
				CollectionButtonUI btn = g.GetComponent<CollectionButtonUI>();
				btn.SetInfo(item);
				buttons.Add(btn);

				matButton.color = Color.white;
				medButton.color = Color.gray;
			}
		}

		Refresh();
	}
}
