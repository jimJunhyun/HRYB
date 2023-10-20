using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryUIController : MonoBehaviour
{
    public List<InventorySlot> InventoryItems = new List<InventorySlot>();

    private VisualElement m_Root;
    private VisualElement m_SlotContainer;

    private void Awake()
    {
        //루트에 UI컴포넌트 저장
        m_Root = GetComponent<UIDocument>().rootVisualElement;

        //루트에서 SlotContainer 찾기
        m_SlotContainer = m_Root.Query("SlotContainer");

        //인벤 생성 후,  SlotContainer의 자식으로 추가
        for (int i = 0; i < 20; i++)
        {
            InventorySlot item = new InventorySlot();

            InventoryItems.Add(item);

            m_SlotContainer.Add(item);
        }
    }
}
