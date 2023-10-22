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
        //새 인벤 슬롯을 만들어 루트에 추가
        Icon = new Image();
        Add(Icon);

        //USS 스타일 속성 추가
        Icon.AddToClassList("slotIcon");
        AddToClassList("slotContainer");

        RegisterCallback<PointerDownEvent>(OnPointerDown);
    }

    private void OnPointerDown(PointerDownEvent evt)
    {
        //마우스 왼쪽 버튼이 아니거나 빈 슬롯인 경우
        if (evt.button != 0 || ItemGuid.Equals(""))
        {
            return;
        }

        //옮기려는 아이템의 아이콘 지우기
        Icon.image = null;

        //드래그 시작
        InventoryUIController.StartDrag(evt.position, this);

    }

    /// <summary>
    /// 아이콘 및 GUID 속성을 설정
    /// </summary>
    /// <param name="item"></param>
    public void HoldItem(ItemDetails item)
    {
        Icon.image = item.Icon.texture;
        ItemGuid = item.GUID;
    }

    /// <summary>
    /// 아이콘 및 GUID 속성을 지우기
    /// </summary>
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
