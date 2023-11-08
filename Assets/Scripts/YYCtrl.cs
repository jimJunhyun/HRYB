using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YYCtrl : MonoBehaviour
{
	const float UISCALE = 100f;

    public float spirit;
    public float yin;
    public float yang;

	public float spinSpeed;
	public float emerSpeed = 5;

	Image blk;
	Image wht;
	Image bgnd;

	float prevYin;
	float prevYang;

	private void Awake()
	{
		prevYin = yin;
		prevYang = yang;

		Image[] imgs = GetComponentsInChildren<Image>();
		bgnd = imgs[0];
		blk = imgs[1];
		wht = imgs[2];
	}

	private void Update()
	{
		float diff = yin / yang > yang / yin ? yin / yang : yang / yin;
		transform.eulerAngles += Vector3.back * spinSpeed * Time.deltaTime * (1 + (diff - 1) * emerSpeed);

		if(prevYin != yin || prevYang != yang)
		{
			prevYang = yang;
			prevYin	= yin;

			float sum = prevYin + prevYang;

			float yinSize = UISCALE * yin / sum;
			float yangSize = UISCALE * yang / sum;
			blk.rectTransform.sizeDelta = Vector2.one * yinSize;
			wht.rectTransform.sizeDelta = Vector2.one * yangSize;
		}
	}
}
