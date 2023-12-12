using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlot : MonoBehaviour
{
	public int value;
	Image quickImage;
	TMP_Text quickText;

	public InventoryItem items;

	private void Start()
	{
		quickImage = GetComponent<Image>();
		quickText = GetComponentInChildren<TMP_Text>();
	}

	private void Update()
	{
		items = GameManager.instance.pinven.inven[value];

		if (items.isEmpty())
		{
			quickImage.sprite = null;
			quickImage.color = Color.clear;
			quickText.text = "";
			return;
		}
		else
		{
			quickImage.sprite = items.info.icon;
			quickImage.color = Color.white;
			quickText.text = items.number.ToString();
		}
	}

	//public void UpdateItem()
	//{
	//	try
	//	{
	//		items = GameManager.instance.pinven.inven[value];


	//		if (items.isEmpty())
	//		{
	//			quickImage.sprite = null;
	//			quickImage.color = Color.clear;
	//			quickText.text = "";
	//			return;
	//		}
	//		else
	//		{
	//			quickImage.sprite = items.info.icon;
	//			quickImage.color = Color.white;
	//			quickText.text = items.number.ToString();
	//		}
	//	}
	//	catch
	//	{
	//		Debug.LogWarning($"{value} Is 없는 번호임");
	//	}


	//}
}
