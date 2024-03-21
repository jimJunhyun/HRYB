using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;


public class HPBar : MonoBehaviour
{
	public float maxHP;
	public LifeModule lf;
	public Slider HPSlider;

	private void Awake()
	{
		
		HPSlider = GetComponent<Slider>();
		lf = GetComponentInParent<LifeModule>(); 
		maxHP =lf.initYinYang.white;
	}

	private void FixedUpdate()
	{		
		HPSlider.value = lf.yy.white / maxHP;

		if(lf.yy.white == maxHP || lf.yy.white == 0)
		{
			this.GetComponentInParent<Canvas>().enabled = false;
		}
		else
		{
			this.GetComponentInParent<Canvas>().enabled = true;
		}
	}
}
