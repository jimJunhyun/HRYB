using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// 끄고킬수있는 UI의 인터페이스 등 구조를 만들자.
/// </summary>

public class UIManager : MonoBehaviour
{
	Canvas canvas;

	public YYCtrl yinYangUI;
	public WXCtrl wuXingUI;
	public AimPointCtrl aimUI;
	public InfoCtrl infoUI;
	public SimpleCrafter crafterUI;
	public QuestUICtrl questUI;
	public InterPrevUI preInterUI;
	public InterProcessUI interingUI;
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
		questUI = canvas.GetComponentInChildren<QuestUICtrl>();
		preInterUI = canvas.GetComponentInChildren<InterPrevUI>();
		interingUI = canvas.GetComponentInChildren<InterProcessUI>();
		invenPanel.SetActive(true);
	}

	private void Start()
	{
		for (int i = 0; i < GameManager.instance.pinven.cap; i++)
		{
			//Debug.Log(GameObject.);
			uis.Add(GameObject.Find(i.ToString()).GetComponentInChildren<InventoryUI>());
		}
		interingUI.SetGaugeValue(0);
		interingUI.Off();
		preInterUI.Off();
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
