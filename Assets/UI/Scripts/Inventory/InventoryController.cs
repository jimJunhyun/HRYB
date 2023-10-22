using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class ItemDetails
{
    public string Name;
    public string GUID;
    public Sprite Icon;
    public bool CanDrop;
}

public enum InventoryChangeType
{
    Pickup,
    Drop
}
public delegate void OnInventoryChangedDelegate(string[] itemGuid, InventoryChangeType change);

public class InventoryController : MonoBehaviour
{
    [SerializeField]
    public List<Sprite> IconSprites;
    private static Dictionary<string, ItemDetails> m_ItemDatabase = new Dictionary<string, ItemDetails>();
    private List<ItemDetails> m_PlayerInventory = new List<ItemDetails>();
    public static event OnInventoryChangedDelegate OnInventoryChanged = delegate { };


    private void Start()
    {
        //아이템 데이터베이스를 플레이어 인벤토리에 추가하고 UI에 일부 아이템이 획득되었음을 알림
        m_PlayerInventory.AddRange(Item.nameDataHashT);
        OnInventoryChanged.Invoke(m_PlayerInventory.Select(x => x.GUID).ToArray(), InventoryChangeType.Pickup);
    }
    
    /// <summary>
    /// GUID를 기반으로 항목 세부 정보 검색
    /// </summary>
    /// <param name="guid">조회할 ID</param>
    /// <returns>아이템 세부 정보</returns>
    public static ItemDetails GetItemByGuid(string guid)
    {
        if (m_ItemDatabase.ContainsKey(guid))
        {
            return m_ItemDatabase[guid];
        }

        return null;
    }
}
