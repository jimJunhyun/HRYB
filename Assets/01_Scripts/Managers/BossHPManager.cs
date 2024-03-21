using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BossHPManager : MonoBehaviour
{

    public GameObject makeHP(string name, LifeModule lf)
	{

		GameObject obj;
		obj = Instantiate(GameManager.instance.pManager.bossHPBar, transform);
		obj.GetComponentInChildren<BossHPBar>().lf = lf;
		obj.GetComponentInChildren<TMP_Text>().text = name;

		return obj;
	}

	public GameObject HideHP(Transform trm)
	{

		GameObject obj;
		obj = Instantiate(GameManager.instance.pManager.HPBar, trm);	

		return obj;
	}
}
