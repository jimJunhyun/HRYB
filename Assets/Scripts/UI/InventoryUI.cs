using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class InventoryUI : MonoBehaviour
{
	public int quickInven;
	private SlotUI[] slotUI;

	QuickSlot[] quickSlot;
	private DragHandler[] dragHandler;

	private void Start()
	{
		if(this.name == "InventoryGroup")
		{
			for (int i = 0; i < GameManager.instance.pinven.cap - quickInven; i++)
			{
				Instantiate(GameManager.instance.pManager.invenSlot, this.transform);
				//Debug.Log($"{i} : {GameManager.instance.pManager.invenSlot}");
			}

			slotUI = GetComponentsInChildren<SlotUI>();
			dragHandler = GetComponentsInChildren<DragHandler>();

			for (int i = 0; i < GameManager.instance.pinven.cap - quickInven; i++)
			{
				slotUI[i].value = quickInven + i;
				dragHandler[i].value = quickInven + i;
			}
		}
		else if(this.name == "Quick InventoryGroup")
		{
			for (int i = 0; i < quickInven; i++)
			{
				Instantiate(GameManager.instance.pManager.invenSlot, this.transform);
				//Debug.Log($"{i} : {GameManager.instance.pManager.invenSlot}");
			}

			slotUI = GetComponentsInChildren<SlotUI>();
			dragHandler = GetComponentsInChildren<DragHandler>();

			for (int i = 0; i < quickInven; i++)
			{
				slotUI[i].value = i;
				dragHandler[i].value = i;
			}
		}
		else if(this.name == "QuickSlot")
		{
			for (int i = 0; i < quickInven; i++)
			{
				Instantiate(GameManager.instance.pManager.quickSlot, this.transform);
				//Debug.Log($"{i} : {GameManager.instance.pManager.quickSlot}");
			}
			quickSlot = GetComponentsInChildren<QuickSlot>();
	
			for (int i = 0; i < quickInven; i++)
			{
				quickSlot[i].value = i;
			}
		}
	}
}
