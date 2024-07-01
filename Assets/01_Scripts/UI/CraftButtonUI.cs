using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;

public class CraftButtonUI : MonoBehaviour
{
	Image img;
	TextMeshProUGUI itemName;
	TextMeshProUGUI desc;


	internal Item connected;
	HashSet<ItemAmountPair> reqItem;
    public void SetInfo(Item i, HashSet<ItemAmountPair> reqItem)
	{
		connected = i;
		this.reqItem = reqItem;
		if(img == null)
		{
			img = transform.Find("Image").GetComponent<Image>();
		}
		if(itemName == null)
		{

			itemName = transform.Find("Name").GetComponent<TextMeshProUGUI>();
		}
		if(desc == null)
		{
			desc = transform.Find("Desc").GetComponent<TextMeshProUGUI>();

		}

		img.sprite = connected.icon;
		itemName.text = connected.MyName;
		desc.text = connected.desc;
	}

	public void OnClick()
	{
		if(connected is Medicines med)
		{
			GameManager.instance.uiManager.medicineDetail.SetInfo(med, reqItem);
		}
	}
}
