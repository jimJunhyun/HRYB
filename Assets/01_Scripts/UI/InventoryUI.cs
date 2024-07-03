using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour, IOpenableWindowUI
{
	public int quickInven;

	public TextMeshProUGUI itemName;
	public TextMeshProUGUI itemDesc;
	public TextMeshProUGUI useInst;
	public Image itemIcon;
	public ItemDetailInfoShower itemDetail;

	[SerializeField]
	private SlotUI[] slotUI;

	[SerializeField]
	private DragHandler[] dragHandler;

	private void Awake()
	{
		itemName = transform.Find("ItemInfo/ItemText").GetComponent<TextMeshProUGUI>();
		itemDesc = transform.Find("ItemInfo/ItemInfo").GetComponent<TextMeshProUGUI>();
		itemIcon = transform.Find("ItemInfo/ItemImg").GetComponent<Image>();
		useInst = transform.Find("ItemInfo/UseInstruction").GetComponent<TextMeshProUGUI>();
		itemDetail = transform.Find("ItemDetail").GetComponent<ItemDetailInfoShower>();
		slotUI = GetComponentsInChildren<SlotUI>();
		dragHandler = GetComponentsInChildren<DragHandler>();

		for (int i = 0; i < slotUI.Length; i++)
		{
			slotUI[i].value = i;
			dragHandler[i].value = i;
		}

		useInst.enabled = false;
	}

	public void OnClose()
	{
		//throw new System.NotImplementedException();
	}

	public void OnOpen()
	{
		for (int i = 0; i < slotUI.Length; i++)
		{
			slotUI[i].value = i;
			dragHandler[i].value = i;
		}
		GameManager.instance.uiManager.UpdateInvenUI();
	}

	public void WhileOpening()
	{
		//throw new System.NotImplementedException();
	}

	public void Refresh()
	{
		if(GameManager.instance.pinven.CurHoldingItem.info != null)
		{
			itemName.text = GameManager.instance.pinven.CurHoldingItem.info.MyName;
			itemDesc.text = GameManager.instance.pinven.CurHoldingItem.info.desc;
			itemIcon.color = Color.white;
			itemIcon.sprite = GameManager.instance.pinven.CurHoldingItem.info.icon;
			if(GameManager.instance.pinven.CurHoldingItem.info is YinyangItem yy)
			{
				itemDetail.SetInfo(yy.processes);
				useInst.enabled = false;
				if(GameManager.instance.pinven.CurHoldingItem.info is Medicines md)
				{
					useInst.enabled = true;
				}
			}
			else
			{
				itemDetail.ResetInfo();
			}
		}
		else
		{
			itemName.text = "";
			itemDesc.text = "";
			itemIcon.sprite = null;
			itemIcon.color = Color.clear;
			itemDetail.ResetInfo();
			
		}
	}
}
