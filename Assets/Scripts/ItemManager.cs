using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public List<Item> items;

	private void Awake()
	{
		items.Add(new Item("³ª¹µ°¡Áö", ItemType.Solid, StackType.numScale, 10));
		items.Add(new YinyangItem("ÀÎ»ï", ItemType.Solid, StackType.numScale, 5, '»ï'));
		items.Add(new YinyangItem("¹°", ItemType.Solid, StackType.numScale, 10, '¼ö'));
		items.Add(new Item("¹åÁÙ", ItemType.Solid, StackType.numScale, 10));
	}
}
