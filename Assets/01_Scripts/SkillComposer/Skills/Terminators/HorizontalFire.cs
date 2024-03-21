using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName  = "Skills/Infos/Range/HorizontalFire")]
public class HorizontalFire : AttackBase
{
	public string rangeName;

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
		DamageArea ar = PoolManager.GetObject("Magic Circle 10", targetPt + (relatedTransform.rotation * offSet), Quaternion.Euler(-90, 0, 0), 3f).GetComponent<DamageArea>();
		ar.SetInfo(self.atk.Damage * damageMult);
		holding = false;
		Debug.Log("띔");
	}

	public override void UpdateStatus()
	{
		if (holding)
		{
			if (!rngDecal)
			{
				rngDecal = PoolManager.GetObject("PlayerDecalCircle", targetPt, Quaternion.Euler(90,0, 0));
			}

			if (Physics.Raycast(relatedTransform.position, relatedTransform.forward, out RaycastHit hit, maxDistance, ~(1 << GameManager.PLAYERLAYER), QueryTriggerInteraction.Ignore))
			{
				targetPt = hit.point;
			}
			else
			{
				targetPt = relatedTransform.position + (relatedTransform.forward * maxDistance);
				if (Physics.Raycast(targetPt, Vector3.down, out RaycastHit hit2, Mathf.Infinity, ~(1 << GameManager.PLAYERLAYER), QueryTriggerInteraction.Ignore))
				{
					targetPt = hit2.point;
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
