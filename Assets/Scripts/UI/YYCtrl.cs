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

	Image blk;
	Image wht;
	
	float diff;

	private void Awake()
	{

		Transform bgnd = transform.GetChild(0);
		Image[] imgs = bgnd.GetComponentsInChildren<Image>();
		blk = imgs[0];
		wht = imgs[1];
	}

	private void Start()
	{
		RefreshValues();
	}

	private void Update()
	{
		
		transform.eulerAngles += Vector3.back * spinSpeed * Time.deltaTime * (1 + (diff - 1) * emerSpeed);

	}

	public void RefreshValues()
	{
		spirit = GameManager.instance.pActor.life.maxSoul;
		yy = GameManager.instance.pActor.life.yywx.yy;

		diff = yy.yinAmt > yy.yangAmt ? yy.yangAmt / yy.yinAmt : yy.yinAmt / yy.yangAmt;

		float sum = yy.yangAmt + yy.yinAmt;

		blk.rectTransform.sizeDelta = Vector2.one * (UICIRCUM * yy.yinAmt / sum);
		wht.rectTransform.sizeDelta = Vector2.one * (UICIRCUM * yy.yangAmt / sum);
	}
}
