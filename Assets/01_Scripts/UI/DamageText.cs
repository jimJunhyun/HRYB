using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{

	

	TextMeshProUGUI txt;
	float curT;

    float amt;
	YYInfo info;
	Vector3 point;

	Color myCol;
	Vector3 originScale;

	float accY = 0;
	float distScaleMult
	{
		get => GameManager.instance.shower.distanceMult.Evaluate(Mathf.Min(((point - Camera.main.transform.position).magnitude / GameManager.instance.shower.maxAmtDist), 1));
	}

	float damageScaleMult;

	float ExistTime
	{
		get => curT / GameManager.instance.shower.showTime;
	}


	bool on = false;

	private void Awake()
	{
		txt = GetComponent<TextMeshProUGUI>();
	}

	public void SetInfo(float dmg, YYInfo inf, Vector3 pt)
	{
		amt = dmg;
		info = inf;
		point = pt;
		curT = 0;
		accY = 0;
		originScale = transform.localScale;
		damageScaleMult = GameManager.instance.shower.damageMult.Evaluate(Mathf.Min(amt / (info == YYInfo.Black ? GameManager.instance.shower.maxAmtBlack : GameManager.instance.shower.maxAmtWhite), 1));
		ShowInfo();
	}

	public void ShowInfo()
	{
		on = true;
		txt.text = amt.ToString();
		switch (info)
		{
			case YYInfo.Black:
				txt.color = GameManager.instance.shower.blackDmgColor.Evaluate((amt / GameManager.instance.shower.maxAmtBlack));
				break;
			case YYInfo.White:
				txt.color = GameManager.instance.shower.whiteDmgColor.Evaluate((amt / GameManager.instance.shower.maxAmtWhite));
				break;
			default:
				break;
		}
		myCol = txt.color;
	}

	public void Resetter()
	{
		transform.localScale = originScale;
		
	}

	public void Updater()
	{
		if (on)
		{
			curT += Time.deltaTime;

			myCol.a = GameManager.instance.shower.alphaMovement.Evaluate(ExistTime);
			txt.color = myCol;

			txt.transform.localScale = originScale * GameManager.instance.shower.scaleMovement.Evaluate(ExistTime) * damageScaleMult * distScaleMult;

			accY += GameManager.instance.shower.yAccelMovement.Evaluate(ExistTime);

			
			transform.position = Camera.main.WorldToScreenPoint(point) + Vector3.up * accY;
			//transform.rotation = Quaternion.LookRotation(Camera.main.transform.position/* - transform.position*/);


			if(ExistTime >= 1)
			{
				on = false;
				Resetter();
			}
		}
	}
}
