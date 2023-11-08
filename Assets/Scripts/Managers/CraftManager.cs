using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftManager : MonoBehaviour
{
    public Crafter crafter;

	private void Awake()
	{
		crafter = new Crafter();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.L))
		{
			CraftWithName("밧줄");
		}
	}

	public void CraftWithName(string name)
	{
		if (crafter.Craft(new ItemAmountPair(name)))
		{
			Debug.Log("잘만듬");
		}
		else
		{
			Debug.Log("실패");
		}
	}

	public void CraftWithItems(params ItemAmountPair[] items)
	{
		Recipe recipe = new Recipe();
		for (int i = 0; i < items.Length; i++)
		{
			recipe.recipe.Add(items[i]);
		}
		crafter.CraftWith(recipe);
	}

	public void SetCurMethod(int mtd)
	{
		crafter.CurMethod = (CraftMethod)mtd;
	}



}
