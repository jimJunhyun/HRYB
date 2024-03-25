using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BossHPManager : MonoBehaviour
{

    public GameObject makeHP(string name, LifeModule lf)
	{

		GameObject obj;

		if(name == "천하대장군" || name == "지하여장군")
		{ 

			obj = Instantiate(GameManager.instance.pManager.jangsungHP, transform);
			obj.GetComponentInChildren<BossHPBar>().lf = lf;
			obj.GetComponentInChildren<TMP_Text>().text = name;
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
