using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoCtrl : MonoBehaviour
{
	Image itemImg;
	TextMeshProUGUI itemName;
	TextMeshProUGUI itemDesc;

	public void Awake()
	{
		itemImg = GameObject.Find("ItemImage").GetComponent<Image>();
		Transform iteminfoP = transform.Find("ItemInfos");
		itemName = iteminfoP.Find("Name").GetComponent<TextMeshProUGUI>();
		itemDesc = iteminfoP.Find("Desc").GetComponent<TextMeshProUGUI>();
	}


	public void OnWithInfo(Item item)
	{
		gameObject.SetActive(true);
		itemImg.sprite= item.icon;
		itemName.text = item.MyName;
		itemDesc.text = item.desc;
	}

	public void Off()
	{
		gameObject.SetActive(false);
	}
}
