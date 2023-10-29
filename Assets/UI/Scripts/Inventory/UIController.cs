//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using UnityEngine.UIElements;

//public class UIController : MonoBehaviour
//{
//    private List<InventorySlot> InventoryItems = new List<InventorySlot>();

//    private VisualElement root;
//    private VisualElement itemSlot;
//    private VisualElement inventory;
//    private static VisualElement itemDragPointer;

//    private static bool isDragging;
//    private static InventorySlot originalSlot;

//    private PlayerInven pInven;
//    private List<InventoryItem> lastInventoryData; // 이전 프레임의 인벤토리 데이터

//    private void Awake()
//    {
//        root = GetComponent<UIDocument>().rootVisualElement;

//        itemDragPointer = root.Query<VisualElement>("ItemDragPointer");
//        inventory = root.Q<VisualElement>("Inventory");
//        itemSlot = root.Q<VisualElement>("SlotContainer");

//        for (int i = 0; i < 25; i++)
//        {
//            InventorySlot item = new InventorySlot();

//            InventoryItems.Add(item);

//            currentInventoryData.Add(pInven.inven[i]);

//            //인벤 25개 만들기                                                                                                                                    
//            itemSlot.Add(item);
//            itemSlot.name = i.ToString();
//            Debug.Log(itemSlot.name);
           
//        }

  
//        //시작할때 인벤 꺼두기
//        inventory.style.display = DisplayStyle.None;

//        pInven = GameManager.instance.pinven;
//        lastInventoryData = new List<InventoryItem>();
//    }

//    // Update is called once per frame
//    void Update()
//    {

//        // 이전 프레임의 데이터와 비교하여 변경 여부를 확인
//        if (!AreInventoryDataEqual(lastInventoryData, currentInventoryData))
//        {
//            // 데이터가 변경되었을 때 UI를 업데이트
//            UpdateUI(currentInventoryData);
//            Debug.Log("인벤 변경 감지");


//            // 변경된 데이터를 이전 데이터로 저장
//            lastInventoryData = new List<InventoryItem>(currentInventoryData);
//        }


//        if (Input.GetKeyDown(KeyCode.I))
//        {
//            if(inventory.style.display == DisplayStyle.None)
//            {
//                inventory.style.display = DisplayStyle.Flex;
//                UnityEngine.Cursor.lockState = CursorLockMode.Confined;
//            }
//            else
//            {
//                inventory.style.display = DisplayStyle.None;
//                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
//            }    
//        }
//    }

//    private bool AreInventoryDataEqual(List<InventoryItem> data1, List<InventoryItem> data2)
//    {
//        if (data1.Count != data2.Count)
//        {
//            return false; // 데이터 개수가 다르면 다른 데이터로 처리
//        }

//        for (int i = 0; i < data1.Count; i++)
//        {
//            // 여기에서 아이템 비교 로직을 추가
//            // 예를 들어, 아이템의 수, 아이템 ID, 아이템 유무 등을 비교
//            if (data1[i].number != data2[i].number || data1[i].info != data2[i].info)
//            {
//                return false; // 다른 데이터로 처리
//            }
//        }

//        return true; // 데이터가 동일하다고 처리
//    }

//    private void UpdateUI(List<InventoryItem> currentInventoryData)
//    {
//        // 여기에서 UI 요소를 업데이트하는 코드를 작성
//        // 예를 들어, 각 인벤토리 슬롯에 대한 업데이트 작업을 수행
//        for (int i = 0; i < InventoryItems.Count; i++)
//        {
//            // 현재 인벤토리 데이터를 사용하여 UI 슬롯 업데이트
//            InventoryItems[i].UpdateSlot(currentInventoryData[i]);
//        }
//    }

//}
