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

	Vector3 targetPt;

	bool holding;
	GameObject rngDecal;



	public override void Operate(Actor self)
	{
		holding = true;
		//base.Operate(self);
	}

	public override void Disoperate(Actor self)
	{
		try
		{
			DamageArea ar = PoolManager.GetObject(prefName, targetPt + (relatedTransform.rotation * offSet), Quaternion.Euler(-90, 0, 0), 3f).GetComponent<DamageArea>();
			ar.SetInfo(self.atk.Damage * damageMult);
		}
		catch
		{
			Debug.LogError("Trying To Create Strange Object.");
		}
		holding = false;
		PoolManager.ReturnObject(rngDecal);
		rngDecal = null;
		Debug.Log("띔");
	}

	public override void UpdateStatus()
	{
		if (holding)
		{
			if (!rngDecal)
			{
				rngDecal = PoolManager.GetObject("PlayerDecalCircle", targetPt, Quaternion.Euler(90,0, 0));
				rngDecal.transform.localScale = Vector3.one * 5;
			}

			if (Physics.Raycast(relatedTransform.position, (relatedTransform.forward + offSet).normalized, out RaycastHit hit, maxDistance, ~(1 << GameManager.PLAYERLAYER), QueryTriggerInteraction.Ignore))
			{
				targetPt = hit.point;
				Debug.DrawRay(relatedTransform.position, (relatedTransform.forward + offSet).normalized * maxDistance, Color.cyan, 20f);
				Debug.Log("가로막힘, ");
			}
			else
			{
				targetPt = relatedTransform.position + (relatedTransform.forward * maxDistance);
				if (Physics.Raycast(targetPt, Vector3.down, out RaycastHit hit2, Mathf.Infinity, ~(1 << GameManager.PLAYERLAYER), QueryTriggerInteraction.Ignore))
				{
					Debug.DrawRay(targetPt, Vector3.down * 1000f, Color.cyan, 1000);
					Debug.Log("바닥 ");
					targetPt = hit2.point;
				}
				else
				{
					Debug.Log("공중, ");
					Debug.DrawRay(targetPt, Vector3.down * 1000f, Color.red, 1000);
				}
			}
			if (rngDecal)
			{
				rngDecal.transform.position = targetPt + (relatedTransform.rotation * offSet) + Vector3.up * 300f;
			}
		}
	}

	internal override void MyDisoperation(Actor self)
	{
		
	}

	internal override void MyOperation(Actor self)
	{
		
	}
}
