using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextShower : MonoBehaviour
{
	public float showTime;

	public float appearNoiseScale = 0.4f;
	public float textInvalidRotation = 140f;

	public Gradient whiteDmgColor;
	public Gradient blackDmgColor;

	public float maxAmtWhite;
	public float maxAmtBlack;

	public float maxAmtDist;


	public AnimationCurve yAccelMovement;
	public AnimationCurve alphaMovement;
	public AnimationCurve scaleMovement;

	public AnimationCurve distanceMult;
	public AnimationCurve damageMult;

	List<DamageText> damageTexts = new List<DamageText>();

	WaitForSeconds waiter;

	private void Awake()
	{
		waiter = new WaitForSeconds(showTime + 0.1f);
	}

	private void Update()
	{
		for (int i = 0; i < damageTexts.Count; i++)
		{
			damageTexts[i].Updater();
		}
	}

	public void GenerateDamageText(Vector3 pos, float damage, YYInfo info)
	{
		damage = Mathf.Round(damage * 100) / 100;
		DamageText dt = PoolManager.GetObject("DamageText", transform).GetComponent<DamageText>();
		pos += (Vector3)UnityEngine.Random.insideUnitCircle * appearNoiseScale;
		dt.SetInfo(damage, info, pos);
		damageTexts.Add(dt);
		StartCoroutine(DelReturnDamageText(dt));
	}

	public void ReturnDamageText(DamageText dt)
	{
		PoolManager.ReturnObject(dt.gameObject);
		damageTexts.Remove(dt);
	}

	IEnumerator DelReturnDamageText(DamageText dt)
	{
		yield return waiter;
		ReturnDamageText(dt);
	}
}