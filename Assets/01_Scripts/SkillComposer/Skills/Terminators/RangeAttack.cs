using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName  = "Skills/Infos/RangeAttack")]
public class RangeAttack : AttackBase
{
	public string rangeName;

	public string prefName;

	public float maxDistance;
	public Vector3 offSet;
	public Vector3 rotationOffset;

	public bool isOffsetRandom;
	public Vector2 randomOffsetRange;

	public Vector2 decalScale;

	public float totalRemainSec;
	public bool isOnce;
	public float checkGap;
	public float checkDur;

	Vector3 targetPt;

	bool holding;
	GameObject rngDecal;

	Vector3 actualOffset;

	public override void Operate(Actor self)
	{
		MyOperation(self);
	}

	public override void Disoperate(Actor self)
	{
		try
		{
			DamageArea ar = PoolManager.GetObject(prefName, targetPt, Quaternion.Euler(rotationOffset)).GetComponent<DamageArea>();
			ar.SetInfo(totalRemainSec ,self.atk.Damage * damageMult, isOnce, checkGap, checkDur, self, statEff);
		}
		catch
		{
			Debug.LogError("Trying To Create Strange Object.");
		}
		MyDisoperation(self);
		Debug.Log("띔");
	}

	public override void UpdateStatus()
	{
		if (holding)
		{
			if (!rngDecal)
			{
				rngDecal = PoolManager.GetObject(rangeName, targetPt, Quaternion.Euler(90,0, 0));
				rngDecal.transform.localScale = Vector3.right * decalScale.x + Vector3.up * decalScale.y + Vector3.forward;
			}
			Vector3 dir = (relatedTransform.rotation * (Vector3.forward * maxDistance + actualOffset)).normalized;
			//Debug.DrawRay(relatedTransform.position, dir, Color.cyan, 1000f);
			if (Physics.Raycast(relatedTransform.position, dir, out RaycastHit hit, maxDistance, ~(1 << GameManager.PLAYERLAYER), QueryTriggerInteraction.Ignore))
			{
				targetPt = hit.point;
				Debug.Log("가로막힘, ");
				//Debug.DrawLine(relatedTransform.position, hit.point, Color.cyan, 1000f);
			}
			else
			{
				targetPt = relatedTransform.position + (dir * maxDistance);
				if (Physics.Raycast(targetPt, Vector3.down, out RaycastHit hit2, Mathf.Infinity, ~((1 << GameManager.PLAYERLAYER) | (1 << GameManager.ENEMYLAYER)), QueryTriggerInteraction.Ignore))
				{
					Debug.DrawRay(targetPt, Vector3.down * 1000f, Color.cyan, 1000);
					Debug.Log("바닥 ");
					targetPt = hit2.point;
				}
			}
			if (rngDecal)
			{
				rngDecal.transform.position = targetPt + Vector3.up * 300f;
				//Debug.Log( name + " RANGEDECAL AT : " + targetPt.ToString());
			}
		}
	}

	internal override void MyDisoperation(Actor self)
	{
		holding = false;
		PoolManager.ReturnObject(rngDecal);
		rngDecal = null;
		actualOffset = Vector3.zero;
	}

	internal override void MyOperation(Actor self)
	{
		holding = true;
		Debug.Log("OPERATED");
		actualOffset = offSet;
		if (isOffsetRandom)
		{
			actualOffset += Random.insideUnitSphere * Random.Range(randomOffsetRange.x, randomOffsetRange.y);
			actualOffset *= Random.Range(0, 1) * 2 - 1;
		}
	}
}
