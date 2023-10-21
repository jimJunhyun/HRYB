using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryUIController : MonoBehaviour
{
    public List<InventorySlot> InventoryItems = new List<InventorySlot>();

    private VisualElement m_Root;
    private VisualElement m_SlotContainer;

    private void Awake()
    {
        //UI툴킷 받아오기
        m_Root = GetComponent<UIDocument>().rootVisualElement;

        m_SlotContainer = m_Root.Query("SlotContainer");

        Item.OnInventoryChanged += GameController_OnInventoryChanged;

    }

    private void GameController_OnInventoryChanged(string[] itemGuid, InventoryChangeType change)
    {
        //Loop through each item and if it has been picked up, add it to the next empty slot
        foreach (string item in itemGuid)
        {
            if (change == InventoryChangeType.Pickup)
            {
                var emptySlot = InventoryItems.FirstOrDefault(x => x.ItemGuid.Equals(""));

                if (emptySlot != null)
                {
                    emptySlot.HoldItem(Item.Equals(item));
                }
            }
        }
    }
}
