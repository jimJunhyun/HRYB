using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;


public class HPBar : MonoBehaviour
{
	public LifeModule lf;
	public Image _hpBar;

	private void Awake()
	{

		_hpBar = GetComponent<Image>();
		lf = GetComponentInParent<LifeModule>(); 
	}

	private void FixedUpdate()
	{
		_hpBar.fillAmount = lf.yy.white / lf.initYinYang.white;

		if(lf.yy.white == lf.initYinYang.white || lf.yy.white <= 0)
		{
			this.GetComponentInParent<Canvas>().enabled = false;
		}
		else
		{
			this.GetComponentInParent<Canvas>().enabled = true;
		}
	}
}
