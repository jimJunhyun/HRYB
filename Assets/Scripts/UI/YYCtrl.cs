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
	Image blk;
	Image wht;

	Slider blkGauge;
	Slider whtGauge;
	
	float diff;

	private void Awake()
	{

		bgnd = transform.GetChild(0);
		Image[] imgs = bgnd.GetComponentsInChildren<Image>();

		blk = imgs[1];
		wht = imgs[2];

		blkGauge = transform.Find("YinGauge").GetComponent<Slider>();
		whtGauge = transform.Find("YangGauge").GetComponent<Slider>();
	}

	private void Start()
	{
		RefreshValues();
	}

	private void Update()
	{
		
		//bgnd.eulerAngles += Vector3.back * spinSpeed * Time.deltaTime * (1 + (1 - diff) * emerSpeed);

	}

	public void RefreshValues()
	{
		spirit = GameManager.instance.pActor.life.maxSoul;
		yy = GameManager.instance.pActor.life.yywx.yy;

		diff = yy.yinAmt > yy.yangAmt ? yy.yangAmt / yy.yinAmt : yy.yinAmt / yy.yangAmt;

		float sum = yy.yangAmt + yy.yinAmt;

		blk.rectTransform.sizeDelta = Vector2.one * (UICIRCUM * yy.yinAmt / sum);
		wht.rectTransform.sizeDelta = Vector2.one * (UICIRCUM * yy.yangAmt / sum);

		blkGauge.value = GameManager.instance.pActor.life.yywx.yy.yinAmt / GameManager.instance.pActor.life.maxSoul;
		whtGauge.value = GameManager.instance.pActor.life.yywx.yy.yangAmt / GameManager.instance.pActor.life.maxSoul;
	}
}
