using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MedicineType
{
	None = -1,

	Liquid,
	Powder,
	Pill,
}

public class Medicines : YinyangItem
{
	public MedicineType type;

	public List<ItemAmountPair> recipe;
	public override float ApplySpeed
	{
		get 
		{
			switch (type)
			{
				case MedicineType.Liquid:
					return applySpeed * 2f;
					break;
				case MedicineType.Powder:
					return applySpeed * 1f;
					break;
				case MedicineType.Pill:
					return applySpeed * 0.8f;
					break;
				default:
					return applySpeed;
					break;
			}
		}
	} //식 필요

    public Medicines(string name, string desc, ItemType iType, int max, Specials used, bool isNewItem, YinyangWuXing yyData)
		:base(name, desc, iType, max, used, isNewItem, yyData, "약")
	{
		this.rarity = ItemRarity.Medicine;
	}

	public override void Use()
	{
		GameManager.instance.pinven.RemoveItem(this);
		base.Use();
	}
}
