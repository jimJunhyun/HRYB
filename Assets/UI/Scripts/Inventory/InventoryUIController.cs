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
    private static VisualElement m_GhostIcon;

    private static bool m_IsDragging;
    private static InventorySlot m_OriginalSlot;

    private void Awake()
    {
        //UI Document component에서 루트 저장
        m_Root = GetComponent<UIDocument>().rootVisualElement;
        m_GhostIcon = m_Root.Query<VisualElement>("GhostIcon");

        //루트에서 SlotContainer 검색
        m_SlotContainer = m_Root.Q<VisualElement>("SlotContainer");

        InventoryController.OnInventoryChanged += Inventorycontroller_OnInventoryChanged;
        m_GhostIcon.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        m_GhostIcon.RegisterCallback<PointerUpEvent>(OnPointerUp);
    }

    /// <summary>
    /// Initiate the drag
    /// </summary>
    /// <param name="position">마우스 위치</param>
    /// <param name="originalSlot">플레이어가 선택한 인벤토리 슬롯</param>
    public static void StartDrag(Vector2 position, InventorySlot originalSlot)
    {
        //추적 변수 설정
        m_IsDragging = true;
        m_OriginalSlot = originalSlot;

        //위치 설정
        m_GhostIcon.style.top = position.y - m_GhostIcon.layout.height / 2;
        m_GhostIcon.style.left = position.x - m_GhostIcon.layout.width / 2;


        m_GhostIcon.style.backgroundImage = InventoryController.GetItemByGuid(originalSlot.ItemGuid).Icon.texture;


        m_GhostIcon.style.visibility = Visibility.Visible;

    }

    /// <summary>
    /// 드래그 
    /// </summary>
    private void OnPointerMove(PointerMoveEvent evt)
    {
        //플레이어가 화면에서 항목을 드래그하는 경우에만 동작
        if (!m_IsDragging)
        {
            return;
        }

        //새 포지션 설정
        m_GhostIcon.style.top = evt.position.y - m_GhostIcon.layout.height / 2;
        m_GhostIcon.style.left = evt.position.x - m_GhostIcon.layout.width / 2;

    }

    /// <summary>
    /// 드래그를 완료하고 항목을 새 슬롯으로 이동해야 하는지 계산
    /// </summary>
    private void OnPointerUp(PointerUpEvent evt)
    {

        if (!m_IsDragging)
        {
            return;
        }

        //Check to see if they are dropping the ghost icon over any inventory slots.
        IEnumerable<InventorySlot> slots = InventoryItems.Where(x => x.worldBound.Overlaps(m_GhostIcon.worldBound));

        //하나 이상 발견
        if (slots.Count() != 0)
        {
            InventorySlot closestSlot = slots.OrderBy(x => Vector2.Distance(x.worldBound.position, m_GhostIcon.worldBound.position)).First();

            //데이터로 새 인벤토리 슬롯 설정하기
            closestSlot.HoldItem(InventoryController.GetItemByGuid(m_OriginalSlot.ItemGuid));

            //기존 슬롯 비우기
            m_OriginalSlot.DropItem();
        }
        //찾지 못함(창 밖으로 드래그)
        else
        {
            m_OriginalSlot.Icon.image = InventoryController.GetItemByGuid(m_OriginalSlot.ItemGuid).Icon.texture;
        }


        //드래그 중인 아이콘 및 데이터 지우기
        m_IsDragging = false;
        m_OriginalSlot = null;
        m_GhostIcon.style.visibility = Visibility.Hidden;
    }

    /// <summary>
    /// Listen for changes to the players inventory and act
    /// </summary>
    /// <param name="itemGuid">Reference ID for the Item Database</param>
    /// <param name="change">Type of change that occurred. This could be extended to handle drop logic.</param>
    private void Inventorycontroller_OnInventoryChanged(string[] itemGuid, InventoryChangeType change)
    {
        //Loop through each item and if it has been picked up, add it to the next empty slot
        foreach (string item in itemGuid)
        {
            if (change == InventoryChangeType.Pickup)
            {
                var emptySlot = InventoryItems.FirstOrDefault(x => x.ItemGuid.Equals(""));

                if (emptySlot != null)
                {
                    emptySlot.HoldItem(InventoryController.GetItemByGuid(item));
                }
            }
        }
    }
}
