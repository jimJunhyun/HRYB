using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Infos/Kick")]
public class Kick : AttackBase
{
	BoxColliderCast caster;

	protected override void OnValidate()
	{
		base.OnValidate();
		caster = relatedTransform.GetComponent<BoxColliderCast>();
		if(caster == null)
		{
			Debug.Log($"NO BOXCOLLIDERCAST FOUND IN : {relatedTransform.name}");
		}
	}

	public override void Operate(Actor self)
	{

	}

	public override void UpdateStatus()
	{
		
	}

	internal override void MyDisoperation(Actor self)
	{
		caster.End();
	}

	internal override void MyOperation(Actor self)
	{
		caster.Now(life =>
		{
			if(life != null)
			{
				DoDamage(life.GetActor(), self);
				string hitEff = (self.atk as PlayerAttack).onNextHits?.Invoke(relatedTransform.gameObject, life);
				CameraManager.instance.ShakeCamFor(0.1f);

				Vector3 effPos = life.transform.GetComponent<Collider>().ClosestPointOnBounds(caster.transform.position);
				if (hitEff != null)
				{
					PoolManager.GetObject(hitEff, effPos, -caster.transform.forward, 2.5f);

				}
				else
				{
					PoolManager.GetObject("Hit 26", effPos, -caster.transform.forward, 2.5f);

				}
			}
			
		});
	}
}
