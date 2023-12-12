using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static GameObject itemBeingDragged;
    public int isDragged = 1;
    private Vector3 startPosition;
	public int value;

    public void OnBeginDrag(PointerEventData eventData)
    {
        itemBeingDragged = gameObject;
		itemBeingDragged.GetComponent<Image>().raycastTarget = false;
        startPosition = transform.position;

        GameManager.instance.uiManager.DragPoint = value;
        //GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
        //transform.parent = GameObject.Find("InventoryPanel").transform;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.position = startPosition;
		itemBeingDragged.GetComponent<Image>().raycastTarget = true;
	}
}
