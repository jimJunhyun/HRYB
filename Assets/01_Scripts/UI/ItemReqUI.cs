using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemReqUI : MonoBehaviour
{
	Image image;
	TextMeshProUGUI itemName;
	TextMeshProUGUI count;
    public bool SetInfo(ItemAmountPair pair)
	{
		bool res;
		if(image == null)
			image = transform.Find("Image").GetComponent<Image>();
		if(itemName == null)
			itemName = transform.Find("Name").GetComponent<TextMeshProUGUI>();
		if(count == null)
			count = transform.Find("Count").GetComponent<TextMeshProUGUI>();

		image.sprite = Item.GetItem<Item>(pair.info.originalName).icon;
		itemName.text = pair.info.MyName;

		System.Text.StringBuilder sb;
		bool usingGlobal = GameManager.GetGlobalSB(out sb);

		if (GameManager.instance.pinven.RemoveItemExamine(pair.info, pair.num))
		{
			sb.Append("<#00ff00>");
			res = true;
		}
		else
		{
			sb.Append("<#ff0000>");
			res = false;
		}
		sb.Append(GameManager.instance.pinven.inven.SumContains(pair.info));
		sb.Append("</color>");
		sb.Append(" / ");
		sb.Append(pair.num);
		count.text = sb.ToString();

		GameManager.ReturnGlobalSB(usingGlobal);
		
		return res;
	}
}
