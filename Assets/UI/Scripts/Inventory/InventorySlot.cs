using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

public class InventorySlot : VisualElement
{
    public Image Icon;
    public string ItemGuid = "";



    public InventorySlot()
    {
        Icon = new Image();
        Add(Icon);

        name = "1";

        //USS 스타일 속성 추가
        Icon.AddToClassList("slotIcon");
        AddToClassList("slotContainer");
    }

    public void UpdateSlot(InventoryItem item)
    {
        if (true)
        {
            // 아이템 이름, 개수, 이미지를 UI에 표시
            Icon.sprite = item.info.icon;
        }
        else
        {
            // 슬롯이 비어있으면 UI를 초기화
            Icon.sprite = null;
        }
    }

    #region UXML
    [Preserve]
    public new class UxmlFactory : UxmlFactory<InventorySlot, UxmlTraits> { }

    [Preserve]
    public new class UxmlTraits : VisualElement.UxmlTraits { }
    #endregion
}
