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

	private void Start()
	{
		Off();
	}


	public void OnWithInfo(Item item)
	{
		gameObject.SetActive(true);
		itemImg.sprite= item.icon;
		itemName.text = item.MyName;
		itemDesc.text = item.desc;
		GameManager.instance.uiManager.crafterUI.Off();
	}

	public void Off()
	{
		GameManager.instance.uiManager.crafterUI.On();
		gameObject.SetActive(false);
	}
}
