using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{



	internal TextMeshProUGUI txt;
	float curT;

	float amt;
	YYInfo info;
	Vector3 point;

	Color myCol;
	Vector3 originScale;

	Vector3 prevPos;

	DamageChannel myChannel;

	float accY = 0;
	float distScaleMult
	{
		get => GameManager.instance.shower.distanceMult.Evaluate(Mathf.Min(((point - Camera.main.transform.position).magnitude / GameManager.instance.shower.maxAmtDist), 1));
	}

	float damageScaleMult;
	float sizeMod;

	float ExistTime
	{
		get => curT / GameManager.instance.shower.showTime;
	}


	bool on = false;

	private void Awake()
	{
		txt = GetComponent<TextMeshProUGUI>();
		originScale = transform.localScale;
	}

	public void SetInfo(float dmg, YYInfo inf, Vector3 pt, DamageChannel channel, float sizeMod)
	{
		amt = dmg;
		info = inf;
		point = pt;
		curT = 0;
		accY = 0;

		myChannel = channel;
		
		damageScaleMult = GameManager.instance.shower.damageMult.Evaluate(Mathf.Min(amt / (info == YYInfo.Black ? GameManager.instance.shower.maxAmtBlack : GameManager.instance.shower.maxAmtWhite), 1));
		prevPos = Camera.main.WorldToScreenPoint(point);

		ShowInfo();
		this.sizeMod = sizeMod;
	}

	public void ShowInfo()
	{
		on = true;
		txt.text = amt.ToString();
		if(myChannel == DamageChannel.Normal)
		{
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
		}
		else
		{
			txt.color = GameManager.instance.shower.channelColors[((int)myChannel)];
		}
		
		myCol = txt.color;
		txt.enabled = false;
	}

	public void Resetter()
	{
		transform.localScale = originScale;

	}

	public void Updater()
	{
		if (on)
		{
			txt.enabled = true;
			Vector3 ptRelativeToCam = point - Camera.main.transform.position;
			

			curT += Time.deltaTime;

			myCol.a = GameManager.instance.shower.alphaMovement.Evaluate(ExistTime);
			txt.color = myCol;

			txt.transform.localScale = originScale * GameManager.instance.shower.scaleMovement.Evaluate(ExistTime) * damageScaleMult * distScaleMult * sizeMod;

			accY += GameManager.instance.shower.yAccelMovement.Evaluate(ExistTime);

			if (Vector3.Dot(ptRelativeToCam.normalized, Camera.main.transform.forward) >= Mathf.Cos(GameManager.instance.shower.textInvalidRotation))
			{
				prevPos = Camera.main.WorldToScreenPoint(point);
			}
			transform.position = prevPos + Vector3.up * accY;

			
			


			if (ExistTime >= 1)
			{
				on = false;
				Resetter();
			}
		}
	}
}