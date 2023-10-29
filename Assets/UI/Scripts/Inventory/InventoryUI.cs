using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public InventoryItem items;
    public Image Iconimg;
    public TMPro.TMP_Text text;

    private void Awake()
    {
        Iconimg = transform.GetComponentInChildren<Image>();
        text = GetComponentInChildren<TMP_Text>();
    }
    private void Start()
    {
        UpdateItem();
    }

    void UpdateItem()
    {
        items = GameManager.instance.pinven.inven[int.Parse(transform.parent.name)];
        Iconimg.sprite = items.info.icon;
        text.text = items.number.ToString();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            UpdateItem();
        }
    }
}
