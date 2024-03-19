using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BossHPManager : MonoBehaviour
{
	GameObject obj;

	public GameObject makeHP(string name, LifeModule lf)
	{
		obj = Instantiate(GameManager.instance.pManager.bossHPBar, transform);
		obj.GetComponentInChildren<BossHPBar>().lf = lf;
		obj.GetComponentInChildren<TMP_Text>().text = name;

		return obj;
	}
}
