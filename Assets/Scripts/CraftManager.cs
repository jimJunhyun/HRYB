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
		if (Input.GetMouseButtonDown(1))
		{
			CraftWithName("πÂ¡Ÿ");
		}
	}

	public void CraftWithName(string name)
	{
		if (crafter.Craft(new ItemAmountPair(name)))
		{
			Debug.Log("¿ﬂ∏∏µÎ");
		}
		else
		{
			Debug.Log("Ω«∆–");
		}
	}

	public void SetBestMethod(int mtd)
	{
		crafter.BestMethod = (CraftMethod)mtd;
	}

}
