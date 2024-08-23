using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MedicineUI : MonoBehaviour, IOpenableWindowUI
{
	[SerializeField]
	private SlotUI[] slotUI;

	[SerializeField]
	List<int> medicineValue;

	public bool isOverlay { get; set; } = false;
	private void Awake()
	{
		slotUI = GetComponentsInChildren<SlotUI>();

		
	}

	public void OnClose()
	{
		
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

		GameManager.instance.uiManager.UpdateInvenUI();
	}

	public void WhileOpening()
	{
		
	}

	public void Refresh()
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

		for (int i = 0; i < slotUI.Length; i++)
		{
			slotUI[i].UpdateItem();
		}
	}
}
