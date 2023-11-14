using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
	Canvas canvas;

	public YYCtrl yinYangUI;
	public WXCtrl wuXingUI;
	public AimPointCtrl aimUI;
	public InfoCtrl infoUI;
	public SimpleCrafter crafterUI;
    public GameObject invenPanel;

    public RectTransform CursorPos;
    public Image Cursor;

    public int DragPoint;
    public int DropPoint;

    public bool isOn = false;

	List<InventoryUI> uis = new List<InventoryUI>();

	private void Awake()
	{
		canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
		invenPanel = canvas.transform.Find("InventoryPanel").gameObject;
		yinYangUI = canvas.GetComponentInChildren<YYCtrl>();
		wuXingUI = canvas.GetComponentInChildren<WXCtrl>();
		aimUI = canvas.GetComponentInChildren<AimPointCtrl>();
		infoUI = canvas.GetComponentInChildren<InfoCtrl>();
		crafterUI = canvas.GetComponentInChildren<SimpleCrafter>();
		invenPanel.SetActive(true);
	}

	private void Start()
	{
		for (int i = 0; i < GameManager.instance.pinven.cap; i++)
		{
			//Debug.Log(GameObject.);
			uis.Add(GameObject.Find(i.ToString()).GetComponentInChildren<InventoryUI>());
		}
		invenPanel.SetActive(false);
	}

	public void OnInven(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			if (!isOn)
			{
				invenPanel.SetActive(true);
				isOn = true;
				GameManager.instance.UnLockCursor();

				UnityEngine.Cursor.visible = true;
				UnityEngine.Cursor.lockState = CursorLockMode.None;
			}
			else
			{
				invenPanel.SetActive(false);
				isOn = false;
				GameManager.instance.LockCursor();
			}
		}
		
	}

	public void UpdateInvenUI()
	{
		for (int i = 0; i < uis.Count; i++)
		{
			uis[i].UpdateItem();
		}
	}

    
}
