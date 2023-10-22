using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;


public class UIController : MonoBehaviour
{
    private VisualElement inventory;
    private static VisualElement itemDragPointer;
    private static bool isDragging;
    private void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        
        itemDragPointer = root.Query<VisualElement>("ItemDragPointer");
        inventory = root.Q<VisualElement>("Inventory");

        inventory.style.display = DisplayStyle.None;
        itemDragPointer.RegisterCallback<PointerMoveEvent>(OnPointerMove);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if(inventory.style.display == DisplayStyle.None)
            {
                inventory.style.display = DisplayStyle.Flex;
            }
            else
            {
                inventory.style.display = DisplayStyle.None;
            }    
        }
    }



    private void OnPointerMove(PointerMoveEvent evt)
    {
        //플레이어가 화면에서 항목을 드래그하는 경우에만 동작
        if (!isDragging)
        {
            return;
        }

        //새 포지션 설정
        itemDragPointer.style.top = evt.position.y - itemDragPointer.layout.height / 2;
        itemDragPointer.style.left = evt.position.x - itemDragPointer.layout.width / 2;

    }

    //private void OnPointerUp(PointerUpEvent evt)
    //{

    //    if (!isDragging)
    //    {
    //        return;
    //    }

    //    //Check to see if they are dropping the ghost icon over any inventory slots.
    //    IEnumerable<InventorySlot> slots = InventoryItems.Where(x => x.worldBound.Overlaps(m_GhostIcon.worldBound));

    //    //하나 이상 발견
    //    if (slots.Count() != 0)
    //    {
    //        InventorySlot closestSlot = slots.OrderBy(x => Vector2.Distance(x.worldBound.position, m_GhostIcon.worldBound.position)).First();

    //        //데이터로 새 인벤토리 슬롯 설정하기
    //        closestSlot.HoldItem(InventoryController.GetItemByGuid(m_OriginalSlot.ItemGuid));

    //        //기존 슬롯 비우기
    //        m_OriginalSlot.DropItem();
    //    }
    //    //찾지 못함(창 밖으로 드래그)
    //    else
    //    {
    //        m_OriginalSlot.Icon.image = InventoryController.GetItemByGuid(m_OriginalSlot.ItemGuid).Icon.texture;
    //    }


    //    //드래그 중인 아이콘 및 데이터 지우기
    //    m_IsDragging = false;
    //    m_OriginalSlot = null;
    //    m_GhostIcon.style.visibility = Visibility.Hidden;
    //}
}
