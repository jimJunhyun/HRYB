using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YYCtrl : MonoBehaviour
{
	const float UICIRCUM = 150;

    float spirit;
    YinYang yy;

	public float spinSpeed;
	public float emerSpeed = 5;

	Transform bgnd;

	Slider blkGauge;
	Slider whtGauge;


	private void Awake()
	{

		blkGauge = transform.Find("YinGauge").GetComponent<Slider>();
		whtGauge = transform.Find("YangGauge").GetComponent<Slider>();
	}

	private void Start()
	{
		RefreshValues();
	}

	//private void Update()
	//{
		
	//	bgnd.eulerAngles += Vector3.back * spinSpeed * Time.deltaTime * (1 + (1 - diff) * emerSpeed);

	//}

	public void RefreshValues()
	{
		yy = GameManager.instance.pActor.life.yy;

		//diff = yy.yinAmt > yy.yangAmt ? yy.yangAmt / yy.yinAmt : yy.yinAmt / yy.yangAmt;

		//float sum = yy.yangAmt + yy.yinAmt;

		//blk.rectTransform.sizeDelta = Vector2.one * (UICIRCUM * yy.yinAmt / sum);
		//wht.rectTransform.sizeDelta = Vector2.one * (UICIRCUM * yy.yangAmt / sum);

		blkGauge.value = yy.black / GameManager.instance.pActor.life.initYinYang.black;
		whtGauge.value = yy.white / GameManager.instance.pActor.life.initYinYang.white;
	}
}
