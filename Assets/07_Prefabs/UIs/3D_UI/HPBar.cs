using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;


public class HPBar : MonoBehaviour
{
	private Image _hpBar;
	public LifeModule lf;

	private void Awake()
	{
		_hpBar = this.GetComponent<Image>();
		lf = GetComponentInParent<LifeModule>(); 
	}

	private void FixedUpdate()
	{		
		_hpBar.fillAmount = lf.yy.white / lf.initYinYang.white;

		if (lf.yy.white == lf.initYinYang.white || lf.yy.white == 0)
		{
			this.GetComponentInParent<Canvas>().enabled = false;
		}
		else
		{
			this.GetComponentInParent<Canvas>().enabled = true;
		}
	}
}
