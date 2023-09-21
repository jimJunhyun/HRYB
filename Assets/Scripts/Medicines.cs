using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MedicineType
{
	None = -1,

	Liquid,
	Powder,
	Ointment,
	Pill,
	Needle,
}

public class Medicines : YinyangItem
{
	public MedicineType type;

	public List<YinyangItem> recipe;
	float intakeMod { get; } //식 필요

    public Medicines(string name, ItemType iType, StackType sType, int max)
		:base(name, iType, sType, max, '약')
	{

	}
}
