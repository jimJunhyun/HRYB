using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
	public int quickInven;
	private SlotUI[] slotUI;
	private DragHandler[] dragHandler;

	private void Start()
	{
		if(this.name == "InventoryGroup")
		{
			for (int i = 0; i <= GameManager.instance.pinven.cap - quickInven; i++)
			{
				Instantiate(GameManager.instance.pManager.invenSlot, this.transform);
				Debug.Log($"{i} : {GameManager.instance.pManager.invenSlot}");
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
				Debug.Log($"{i} : {GameManager.instance.pManager.invenSlot}");
			}

			slotUI = GetComponentsInChildren<SlotUI>();
			dragHandler = GetComponentsInChildren<DragHandler>();

			for (int i = 0; i < quickInven; i++)
			{
				slotUI[i].value = i;
				dragHandler[i].value = i;
			}
		}
		
	}
}
