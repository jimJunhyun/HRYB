using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FusionUI : MonoBehaviour, IOpenableWindowUI
{
	[SerializeField]
	private SlotUI[] slotUI;

	[SerializeField]
	List<int> medicineValue;

	Transform content;

	List<GameObject> buttons = new List<GameObject>();

	const string CRAFTBUTTON = "CraftableMedicine";

	private void Awake()
	{
		slotUI = GetComponentsInChildren<SlotUI>();

		content = transform.Find("Invens/MedicineView/Viewport/Content");
	}

	public void OnClose()
	{
		for (int i = 0; i < buttons.Count; i++)
		{
			PoolManager.ReturnObject(buttons[i]);
		}
		buttons.Clear();
		GameManager.instance.uiManager.medicineDetail.Close();
	}

	public void OnOpen()
	{
		for (int i = 0; i < slotUI.Length; i++)
		{
			slotUI[i].value = -1;
		}

		medicineValue.Clear();

		for (int i = 0; i < GameManager.instance.pinven.cap; i++)
		{
			if (GameManager.instance.pinven.inven[i].info is YinyangItem)
			{
				medicineValue.Add(i);
			}

		}
		for (int i = 0; i < medicineValue.Count; i++)
		{
			slotUI[i].value = medicineValue[i];
		}
		
		foreach (Recipe item in Crafter.recipeItemTable.Keys)
		{
			//bool craftable = true;
			//foreach (ItemAmountPair recipeAtom in item.recipe)
			//{
			//	craftable &= GameManager.instance.pinven.RemoveItemExamine(recipeAtom.info, recipeAtom.num);
			//}
			//if (craftable)
			//{
			GameObject g = PoolManager.GetObject(CRAFTBUTTON, content);
			CraftButtonUI btn = g.GetComponent<CraftButtonUI>();
			btn.SetInfo(((ItemAmountPair)Crafter.recipeItemTable[item]).info as Medicines, item.recipe);
			buttons.Add(g);
			//}
		}

		GameManager.instance.uiManager.UpdateInvenUI();
	}

	public void WhileOpening()
	{

	}

	public void Refresh()
	{
		for (int i = 0; i < slotUI.Length; i++)
		{
			slotUI[i].UpdateItem();
		}
	}

}
