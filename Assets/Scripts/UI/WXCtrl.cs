using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WXCtrl : MonoBehaviour
{
    public const float DEFAULTSIZE = 30; //0일때
	public const float MAXSIZE = 100; //최대치일때

	Image wood;
	Image fire;
	Image earth;
	Image metal;
	Image water;

	WuXing wx;
	WuXing wxLimit;

	private void Awake()
	{
		wood = transform.Find("Wood").GetComponent<Image>();
		fire = transform.Find("Fire").GetComponent<Image>();
		earth = transform.Find("Earth").GetComponent<Image>();
		metal = transform.Find("Metal").GetComponent<Image>();
		water = transform.Find("Water").GetComponent<Image>();
	}

	private void Start()
	{
		RefreshValues();
	}

	public void RefreshValues()
	{
		wx = GameManager.instance.pActor.life.yywx.wx;
		wxLimit = GameManager.instance.pActor.life.limitation;


		wood.rectTransform.sizeDelta = Vector3.one * Mathf.Max(DEFAULTSIZE ,MAXSIZE * Mathf.Clamp01(wx[((int)WXInfo.Wood)] / wxLimit[((int)WXInfo.Wood)]));
		fire.rectTransform.sizeDelta = Vector3.one * Mathf.Max(DEFAULTSIZE ,MAXSIZE * Mathf.Clamp01(wx[((int)WXInfo.Fire)] / wxLimit[((int)WXInfo.Fire)]));
		earth.rectTransform.sizeDelta = Vector3.one * Mathf.Max(DEFAULTSIZE ,MAXSIZE * Mathf.Clamp01(wx[((int)WXInfo.Earth)] / wxLimit[((int)WXInfo.Earth)]));
		metal.rectTransform.sizeDelta = Vector3.one * Mathf.Max(DEFAULTSIZE ,MAXSIZE * Mathf.Clamp01(wx[((int)WXInfo.Metal)] / wxLimit[((int)WXInfo.Metal)]));
		water.rectTransform.sizeDelta = Vector3.one * Mathf.Max(DEFAULTSIZE ,MAXSIZE * Mathf.Clamp01(wx[((int)WXInfo.Water)] / wxLimit[((int)WXInfo.Water)]));
	}
}
