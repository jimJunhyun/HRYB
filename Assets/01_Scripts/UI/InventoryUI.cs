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
	public TextMeshProUGUI itemUseDesc;
	public TextMeshProUGUI useInst;
	public TextMeshProUGUI invenState;
	public Image itemIcon;
	public Image itemIconBgnd;
	public ItemDetailInfoShower itemDetail;

	public StatTxtUI[] statUI;
	[SerializeField]
	private SlotUI[] slotUI;

	[SerializeField]
	private DragHandler[] dragHandler;
	public bool isOverlay { get; set; } = false;
	private void Awake()
	{
		itemName = transform.Find("ItemInfo/ItemText").GetComponent<TextMeshProUGUI>();
		itemDesc = transform.Find("ItemInfo/ItemInfo").GetComponent<TextMeshProUGUI>();
		itemUseDesc = transform.Find("ItemInfo/ItemUseInfo").GetComponent<TextMeshProUGUI>();
		itemIcon = transform.Find("ItemInfo/ItemImg").GetComponent<Image>();
		itemIconBgnd = transform.Find("ItemInfo/ItemImgBgnd").GetComponent<Image>();
		useInst = transform.Find("ItemInfo/UseInstruction").GetComponent<TextMeshProUGUI>();
		invenState = transform.Find("Inventory/FillText").GetComponent<TextMeshProUGUI>();
		itemDetail = transform.Find("ItemDetail").GetComponent<ItemDetailInfoShower>();
		slotUI = GetComponentsInChildren<SlotUI>();
		statUI = GetComponentsInChildren<StatTxtUI>();
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
		for (int i = 0; i < statUI.Length; i++)
		{
			statUI[i].DoRefresh();
		}
		invenState.text = $"{GameManager.instance.pinven.inven.InvenCount} / {GameManager.instance.pinven.cap - PlayerInven.QUICKSIZE}";
		if(GameManager.instance.pinven.CurHoldingItem.info != null)
		{
			itemName.text = GameManager.instance.pinven.CurHoldingItem.info.MyName;
			itemDesc.text = GameManager.instance.pinven.CurHoldingItem.info.desc;
			itemIcon.color = Color.white;
			itemIconBgnd.color = Color.white;
			itemIcon.sprite = GameManager.instance.pinven.CurHoldingItem.info.icon;
			if(GameManager.instance.pinven.CurHoldingItem.info is YinyangItem yy)
			{
				itemDetail.SetInfo(yy.processes);
				useInst.enabled = false;
				itemUseDesc.text = "사용 불가";
				if(GameManager.instance.pinven.CurHoldingItem.info is Medicines md)
				{
					useInst.enabled = true;
					itemUseDesc.text = md.onUse.ToString();
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
			itemUseDesc.text = "";
			itemIcon.sprite = null;
			itemIcon.color = Color.clear;
			itemIconBgnd.color = Color.clear;
			itemDetail.ResetInfo();
		}
	}
}
