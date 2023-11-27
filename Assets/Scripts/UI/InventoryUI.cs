using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


public class InventoryUI : MonoBehaviour,IDropHandler, IPointerClickHandler
{
    public InventoryItem items;
    public Image Iconimg;
    public TMPro.TMP_Text text;

    int dropPoint;

    private void Awake()
    {
        Iconimg = transform.GetComponentInChildren<Image>();
        text = GetComponentInChildren<TMP_Text>();
    }
 
    public void UpdateItem()
    {
        items = GameManager.instance.pinven.inven[int.Parse(transform.parent.name)];

        if(items.isEmpty())
        {
            Iconimg.sprite = null;
			Iconimg.color = Color.clear;
            text.text = "";
            return;
        }
        else
        {
            Iconimg.sprite = items.info.icon;
			Iconimg.color = Color.white;
			text.text = items.number.ToString();
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("부모 이름" + transform.parent.name);
        if (int.TryParse(transform.parent.name, out dropPoint))
		{
			GameManager.instance.uiManager.DropPoint = dropPoint;
			Debug.Log("드롭" + GameManager.instance.uiManager.DropPoint);


			GameManager.instance.pinven.Move(GameManager.instance.uiManager.DragPoint, GameManager.instance.uiManager.DropPoint, GameManager.instance.pinven.inven[GameManager.instance.uiManager.DragPoint].number);
		}

		
    }

	public void OnPointerClick(PointerEventData eventData)
	{
		if(items.info != null)
		{
			Debug.Log("YESITEM");
			GameManager.instance.uiManager.infoUI.OnWithInfo(items.info);
			GameManager.instance.uiManager.crafterUI.Off();
		}
		else
		{

			GameManager.instance.uiManager.infoUI.Off();
			GameManager.instance.uiManager.crafterUI.On();
			Debug.Log("NOITEM");
		}
	}
}
