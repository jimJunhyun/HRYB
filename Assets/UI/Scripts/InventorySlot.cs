using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventorySlot : VisualElement
{
    public Image Icon;
    public string ItemGuid = "";

    public InventorySlot()
    {
        Icon = new Image();
        Add(Icon);

        Icon.AddToClassList("slotIcon");
        AddToClassList("slotContainer");
    }
}
