using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
				obj = Instantiate(GameManager.instance.saver.pManager.jangsungHP, transform);
				jangsungHP = true;

				GetComponentInChildren<BossHPBar>().boyLf = GameObject.Find("JangSungMen").GetComponent<LifeModule>();
				GetComponentInChildren<BossHPBar>().girlLf = GameObject.Find("JangSungGirl").GetComponent<LifeModule>();
				GetComponentInChildren<BossHPBar>().boyHp = GameObject.Find("Boyhp").GetComponent<Image>();
				GetComponentInChildren<BossHPBar>().girlHp = GameObject.Find("Girlhp").GetComponent<Image>();
				GetComponentInChildren<BossHPBar>().boyWhiteAdequity = GameObject.Find("BoyWhiteBack").GetComponent<Image>();
				GetComponentInChildren<BossHPBar>().girlWhiteAdequity = GameObject.Find("GirlWhiteBack").GetComponent<Image>();

			}
		}
		else
		{
			obj = Instantiate(GameManager.instance.saver.pManager.bossHPBar, transform);
			//obj.GetComponentInChildren<BossHPBar>().lf = lf;
			obj.GetComponentInChildren<TMP_Text>().text = name;
		}

		
		return obj;
	}

	public GameObject HideHP(Transform trm)
	{

		GameObject obj;
		obj = Instantiate(GameManager.instance.saver.pManager.HPBar, trm);	

		return obj;
	}
}
