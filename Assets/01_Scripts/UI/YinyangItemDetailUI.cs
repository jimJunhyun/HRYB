using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;



public class YinyangItemDetailUI : MonoBehaviour
{
	ItemCollection cur;

	Image image;
	
	TextMeshProUGUI itemName;
	TextMeshProUGUI itemDesc;
	
	UIPolygon statPolygon;
	
	Image moistGauge;
	Image poisonGauge;

	GameObject maskItem;
	GameObject maskGroup;
	List<GameObject> buttons = new List<GameObject>();

	Transform content;

	TextMeshProUGUI abilityTxt;

	public const string MAKABLEITEM = "GettableItem";
	private void Awake()
	{
		gameObject.SetActive(false);
	}
	public void SetInfo(ItemCollection item)
	{
		
		for (int i = 0; i < buttons.Count; i++)
		{
			PoolManager.ReturnObject(buttons[i]);
		}
		buttons.Clear();

		if (image == null)
		{
			image = transform.Find("Center Section/Frame 2/ItemImg").GetComponent<Image>();
		}
		if (itemName == null)
		{
			itemName = transform.Find("Center Section/Frame 1/Name/ItemName").GetComponent<TextMeshProUGUI>();
		}
		if(itemDesc == null)
		{
			itemDesc = transform.Find("Center Section/Frame 3/DescText").GetComponent<TextMeshProUGUI>();
		}
		if (content == null)
		{
			content = transform.Find("Right Section/Frame 2/ItemBack/ResultView/Viewport/Content");
		}
		if (statPolygon == null)
		{
			statPolygon = transform.Find("Right Section/Frame 1/StatBack/StatPolygon").GetComponent<UIPolygon>();
		}
		if(moistGauge == null)
		{
			moistGauge = transform.Find("Right Section/Frame 1/StatBack/MoistGauge").GetComponent<Image>();
		}
		if(poisonGauge == null)
		{
			poisonGauge = transform.Find("Right Section/Frame 1/StatBack/PoisonGauge").GetComponent<Image>();
		}
		if (maskItem == null)
		{
			maskItem = transform.Find("Center Section/Frame 2/ItemImg/ItemMask").gameObject;
		}
		if (maskGroup == null)
		{
			maskGroup = transform.Find("Right Section/Frame 1/StatBack/MaskGroup").gameObject;
		}
		if(abilityTxt == null)
		{
			abilityTxt = transform.Find("Right Section/Frame 3/ExpBack/Viewport/EffTxts").GetComponent<TextMeshProUGUI>();
		}


		if (item == null)
		{
			//gameObject.SetActive(false);
			return;
		}
		//
		//gameObject.SetActive(true);



		image.sprite = item.myItem.icon;
		itemName.text = item.discovered ? item.myItem.MyName : "???";
		itemDesc.text = item.discovered ? item.myItem.desc : "???";
		statPolygon.VerticesDistances[0] = ((YinyangItem)item.myItem).detailParams[DetailParameter.Sweet];
		statPolygon.VerticesDistances[1] = ((YinyangItem)item.myItem).detailParams[DetailParameter.Sour];
		statPolygon.VerticesDistances[2] = ((YinyangItem)item.myItem).detailParams[DetailParameter.Bitter];
		statPolygon.VerticesDistances[3] = ((YinyangItem)item.myItem).detailParams[DetailParameter.Salty];
		statPolygon.VerticesDistances[4] = ((YinyangItem)item.myItem).detailParams[DetailParameter.Spicy];
		statPolygon.SetVerticesDirty();
		moistGauge.fillAmount = ((YinyangItem)item.myItem).detailParams[DetailParameter.Moist];
		poisonGauge.fillAmount = ((YinyangItem)item.myItem).detailParams[DetailParameter.Poison];
		if(item.myItem is Medicines m && m.onUse != null)
		{
			abilityTxt.text = m.onUse.ToString();
		}
		else
		{
			abilityTxt.text = "없음";
		}

		foreach (Item i in item.ResultItems)
		{
			

			if(i is YinyangItem)
			{
				GameObject g = PoolManager.GetObject(MAKABLEITEM, content);
				CollectionButtonUI btn = g.GetComponent<CollectionButtonUI>();
				btn.SetInfo(GameManager.instance.saver.pedia.materialCollections[(YinyangItem)i]);
				buttons.Add(g);
			}
		}

		if (item.discovered)
		{
			maskItem.SetActive(false);
			maskGroup.SetActive(false);
		}
		else
		{
			maskItem.SetActive(true);
			maskGroup.SetActive(true);
		}

		cur = item;

	}

	public void RefreshInfo()
	{
		SetInfo(cur);
	}
}
