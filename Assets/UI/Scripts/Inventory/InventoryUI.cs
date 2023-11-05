using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


public class InventoryUI : MonoBehaviour,IDropHandler
{
    public InventoryItem items;
    public Image Iconimg;
    public TMPro.TMP_Text text;
    public GameObject item;

    int dropPoint;

    private void Awake()
    {
        Iconimg = transform.GetComponentInChildren<Image>();
        text = GetComponentInChildren<TMP_Text>();
        item = GetComponent<GameObject>();
    }
 
    void UpdateItem()
    {
        items = GameManager.instance.pinven.inven[int.Parse(transform.parent.name)];

        if(items.isEmpty())
        {
            Iconimg.sprite = null;
            text.text = "";
            return;
        }
        else
        {
            Iconimg.sprite = items.info.icon;
            text.text = items.number.ToString();
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            UpdateItem();
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("부모 이름" + transform.parent.name);
        dropPoint = int.Parse(transform.parent.name);

        UIManager.instance.DropPoint = dropPoint;
        Debug.Log("드롭" + UIManager.instance.DropPoint);

        GameManager.instance.pinven.Move(UIManager.instance.DragPoint, UIManager.instance.DropPoint, GameManager.instance.pinven.inven[UIManager.instance.DragPoint].number);
    }
}
