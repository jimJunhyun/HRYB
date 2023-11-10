using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{

    public RectTransform CursorPos;
    public Image Cursor;

    public int DragPoint;
    public int DropPoint;

    public GameObject Panel;
    public bool isOn = false;

	List<InventoryUI> uis = new List<InventoryUI>();
	private void Start()
	{
		for (int i = 0; i < GameManager.instance.pinven.cap; i++)
		{
			uis.Add(GameObject.Find(i.ToString()).GetComponent<InventoryUI>());
		}
	}

	public void OnInven(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			if (!isOn)
			{
				Panel.SetActive(true);
				isOn = true;
				GameManager.instance.UnLockCursor();

				UnityEngine.Cursor.visible = true;
				UnityEngine.Cursor.lockState = CursorLockMode.None;
			}
			else
			{
				Panel.SetActive(false);
				isOn = false;
				GameManager.instance.LockCursor();
			}
		}
		
	}

	public void UpdateInvenUI()
	{

	}

    
}
