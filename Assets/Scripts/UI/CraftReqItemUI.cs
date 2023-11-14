using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftReqItemUI : MonoBehaviour
{
	public const float ICONSCALE = 50;
	Image icon;
	TextMeshProUGUI itemName;
	TextMeshProUGUI itemReqNum;

	private void Awake()
	{
		if((icon = transform.Find("IconImg"). GetComponent<Image>()) == null)
		{
			GameObject g = new GameObject("IconBgnd");
			GameObject g1 = new GameObject("IconImg");
			Image iconB;
			(iconB = g.AddComponent<Image>()).sprite = GameManager.instance.uiBase;
			iconB.rectTransform.anchorMin = Vector2.zero;
			iconB.rectTransform .anchorMax = Vector2.one;
			iconB.rectTransform.sizeDelta = Vector2.zero;
			icon = g1.AddComponent<Image>();
			icon.rectTransform.sizeDelta = Vector2.one * ICONSCALE;
		}
		if((itemName = transform.Find("ItemName").GetComponent<TextMeshProUGUI>() ) == null)
		{
			GameObject g = new GameObject("ItemName");
			itemName = g.AddComponent<TextMeshProUGUI>();
			itemName.rectTransform.sizeDelta = new Vector2(120, 40);
			itemName.rectTransform.localPosition = new Vector2(90, 17);
			itemName.rectTransform.anchoredPosition = new Vector2(90, 17);
		}
		if((itemReqNum = transform.Find("ItemAmt").GetComponent<TextMeshProUGUI>()) == null)
		{
			GameObject g = new GameObject("ItemAmt");
			itemReqNum = g.AddComponent<TextMeshProUGUI>();
			itemReqNum.rectTransform.sizeDelta = new Vector2(75, 35);
			itemReqNum.rectTransform.localPosition = new Vector2(120, -20);
			itemReqNum.rectTransform.anchoredPosition = new Vector2(120, -20);
		}
	}

	public void SetInfo(string name, int reqNum, int curNum)
	{
		gameObject.SetActive(true);
		icon.sprite = (Item.nameDataHashT[name] as Item).icon;
		itemName.text = (Item.nameDataHashT[name] as Item).MyName;
		itemReqNum.text = $"{curNum}/{reqNum}";
	}

	public void ResetInfo()
	{
		gameObject.SetActive(false);
	}
}
