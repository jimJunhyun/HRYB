using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

public class InventorySlot : VisualElement
{
    public Image Icon;
    public string ItemGuid = "";

    public InventorySlot()
    {
        //새로운 인벤 칸 만들고 넣기
        Icon = new Image();
        Add(Icon);

        //UI툴킷에 적용시키기
        Icon.AddToClassList("slotIcon");
        AddToClassList("slotContainer");


    }

    public void HoldItem(Item item)
    {
        Icon.image = item.icon.texture;
        ItemGuid = item.myName;
    }

    public void DropItem()
    {
        ItemGuid = "";
        Icon.image = null;
    }

    #region UXML
    [Preserve]
    public new class UxmlFactory : UxmlFactory<InventorySlot, UxmlTraits> { }

    [Preserve]
    public new class UxmlTraits : VisualElement.UxmlTraits { }
    #endregion
}
