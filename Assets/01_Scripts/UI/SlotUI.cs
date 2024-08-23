using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


public class SlotUI : MonoBehaviour,IDropHandler, IPointerClickHandler
{
    public InventoryItem items;
    public Image Iconimg;
    public Image frame;
    public TMPro.TMP_Text text;
	public int value;
    int dropPoint;

	Button btn;

    private void Awake()
    {
        Iconimg = transform.GetComponentInChildren<Image>();
        text = transform.parent.GetComponentInChildren<TMP_Text>();
		frame = transform.parent.Find("Frame").GetComponent<Image>();

		btn = GetComponent<Button>();
		if (btn)
		{
			btn.onClick.AddListener(OnMyButtonPressed);
		}
    }
 
    public void UpdateItem()
    {
		try
		{
			if(value < 0)
			{
				Iconimg.sprite = null;
				Iconimg.color = Color.clear;
				text.text = "";
				frame.color = Color.white;
				if (btn)
				{
					btn.interactable = false;
				}
				return;
			}
			Debug.Log(value + "번째 슬롯 갱신");
			items = GameManager.instance.pinven.inven[value];

			if (items.isEmpty())
			{
				Iconimg.sprite = null;
				Iconimg.color = Color.clear;
				text.text = "";
				frame.color = Color.gray;
				if (btn)
				{
					btn.interactable = false;
				}
				return;
			}
			else
			{
				//Debug.LogError("안빔");
				Iconimg.sprite = items.info.icon;
				Iconimg.color = Color.white;
				text.text = items.number.ToString();
				if (btn)
				{
					btn.interactable = true;
				}
				if (GameManager.instance.pinven.curHolding == value)
				{
					frame.color = Color.red;
				}
				else
				{
					frame.color = Color.white;
				}
			}
		}
		catch (System.Exception e)
		{
			Debug.LogWarning($"{e.Message} : {value} Is 없는 번호임");
			Iconimg.sprite = null;
			Iconimg.color = Color.clear;
			if (btn)
			{
				btn.interactable = false;
			}
			text.text = "";
		}


    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("부모 이름" + transform.parent.name);
        if (int.TryParse(value.ToString(), out dropPoint))
		{
			GameManager.instance.uiManager.DropPoint = dropPoint;
			Debug.Log("드롭" + GameManager.instance.uiManager.DropPoint);


			GameManager.instance.pinven.Move(GameManager.instance.uiManager.DragPoint, GameManager.instance.uiManager.DropPoint, GameManager.instance.pinven.inven[GameManager.instance.uiManager.DragPoint].number);

			GameManager.instance.uiManager.UpdateInvenUI();

		}

		
    }

	public void OnPointerClick(PointerEventData eventData)
	{
		if(items.info != null)
		{
			Debug.Log("YESITEM");
			//GameManager.instance.uiManager.infoUI.OnWithInfo(items.info);
			//GameManager.instance.uiManager.crafterUI.Off();
		}
		else
		{
			//GameManager.instance.uiManager.infoUI.Off();
			//GameManager.instance.uiManager.crafterUI.On();
			Debug.Log("NOITEM");
		}
	}

	public void OnMyButtonPressed()
	{
		GameManager.instance.pinven.Hold(value);
	}
}
