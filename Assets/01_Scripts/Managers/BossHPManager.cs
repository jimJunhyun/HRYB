using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BossHPManager : MonoBehaviour
{
	public bool jangsungHP = false;
	
	GameObject obj;

	public GameObject makeHP(string name, LifeModule lf)
	{
		if (name == "천하대장군" || name == "지하여장군")
		{
			Debug.Log("장승이다!");

			if (!jangsungHP)
			{
				obj = Instantiate(GameManager.instance.pManager.jangsungHP, transform);
				jangsungHP = true;
				GameObject.Find("Boy").GetComponentInChildren<BossHPBar>().lf = GameObject.Find("JangSung").GetComponent<LifeModule>();
				GameObject.Find("Girl").GetComponentInChildren<BossHPBar>().lf = GameObject.Find("JangSungGirl").GetComponent<LifeModule>();

			}
		}
		else
		{
			obj = Instantiate(GameManager.instance.pManager.bossHPBar, transform);
			obj.GetComponentInChildren<BossHPBar>().lf = lf;
			obj.GetComponentInChildren<TMP_Text>().text = name;
		}

		
		return obj;
	}

	public GameObject HideHP(Transform trm)
	{

		GameObject obj;
		obj = Instantiate(GameManager.instance.pManager.HPBar, trm);	

		return obj;
	}
}
