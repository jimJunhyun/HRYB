using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MedicineDetailUI : MonoBehaviour //이미지, 이름, 필요아이템
{
	internal Medicines cur;
	HashSet<ItemAmountPair> reqs;

	Image image;
	TextMeshProUGUI itemName;
	Transform content;
	Button craft;
	
	List<ItemReqUI> reqItems = new List<ItemReqUI>();

	const string REQITEM = "ReqItem";
	private void Awake()
	{
		gameObject.SetActive(false);
	}
	public void SetInfo(Medicines med, HashSet<ItemAmountPair> reqItem, bool isRefresh = false)
	{
		if(image == null)
		{
			image = transform.Find("Image").GetComponent<Image>();
		}
		if(itemName == null)
		{
			itemName = transform.Find("Name").GetComponent<TextMeshProUGUI>();
		}
		if(content == null)
		{
			content = transform.Find("Req/Viewport/Content");
		}
		if(craft == null)
		{
			craft = transform.Find("Craft").GetComponent<Button>();
		}


		reqs = reqItem;
		for (int i = 0; i < reqItems.Count; i++)
		{
			PoolManager.ReturnObject(reqItems[i].gameObject);
		}
		reqItems.Clear();

		if(med == null || (cur == med && !isRefresh))
		{
			Close();
		}
		else
		{
			gameObject.SetActive(true);
			
			image.sprite = med.icon;
			itemName.text = med.MyName;
			bool res = true;
			foreach (var item in reqItem)
			{
				ItemReqUI rq = PoolManager.GetObject(REQITEM, content).GetComponent<ItemReqUI>();
				res &= rq.SetInfo(item);
				reqItems.Add(rq);
			}

			craft.interactable = res;
			craft.onClick.RemoveAllListeners();
			craft.onClick.AddListener(() => GameManager.instance.craftManager.CraftWithName(med.MyName));
			cur = med;
		}

	}

	public void Close()
	{
		gameObject.SetActive(false);
		cur = null;
	}

	public void RefreshInfo()
	{
		SetInfo(cur, reqs, true);
	}
}
