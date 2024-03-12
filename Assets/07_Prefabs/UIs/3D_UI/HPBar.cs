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
	BoxCollider Box;
	public float HPBarHeight = 20;


	private void Awake()
	{
		Box = GetComponentInParent<BoxCollider>();
		
		HPSlider = GetComponent<Slider>();
		lf = GetComponentInParent<LifeModule>(); 
		maxHP =lf.initYinYang.white;

		//GetComponent<RectTransform>().anchoredPosition.Set(GetComponent<RectTransform>().anchoredPosition.x, Box.size.y + 20); //몬스터 키에 맞게 UI위치 지정
	}

	private void FixedUpdate()
	{		
		HPSlider.value = lf.yy.white / maxHP;
	}
}
