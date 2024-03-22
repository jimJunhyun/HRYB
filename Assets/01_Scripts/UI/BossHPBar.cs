using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHPBar : MonoBehaviour
{
	public LifeModule lf;
	Image _hpBar;


	private void Awake()
	{
		_hpBar = GetComponent<Image>();
	}

	void Update()
	{
		_hpBar.fillAmount = lf.yy.white / lf.initYinYang.white;
	}
	
}
