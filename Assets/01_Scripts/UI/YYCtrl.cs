using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YYCtrl : MonoBehaviour
{
	const float UICIRCUM = 150;

    YinYang yy;

	public float spinSpeed;
	
	Transform bgnd;

	Image blkGauge;
	Image whtGauge;


	private void Awake()
	{

		blkGauge = GameObject.Find("YangFill").GetComponent<Image>();
		whtGauge = GameObject.Find("YinFill").GetComponent<Image>();
		bgnd = transform.GetChild(0);
	}

	private void Start()
	{
		RefreshValues();
	}

	private void Update()
	{

		bgnd.eulerAngles += Vector3.back * spinSpeed * Time.deltaTime;

	}

	public void RefreshValues()
	{
		yy = GameManager.instance.pActor.life.yy;

		//diff = yy.yinamt > yy.yangamt ? yy.yangamt / yy.yinamt : yy.yinamt / yy.yangamt;

		//float sum = yy.yangamt + yy.yinamt;

		//blk.recttransform.sizedelta = vector2.one * (uicircum * yy.yinamt / sum);
		//wht.recttransform.sizedelta = vector2.one * (uicircum * yy.yangamt / sum);

		blkGauge.fillAmount = yy.black / GameManager.instance.pActor.life.initYinYang.black;
		whtGauge.fillAmount = yy.white / GameManager.instance.pActor.life.initYinYang.white;
	}
}
