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

	public override void UpdateStatus()	// ?? 스킬 시전중
	{
		
	}

	internal override void MyDisoperation(Actor self) // 사라질 때
	{
		caster.End();
	}

	internal override void MyOperation(Actor self) // 애니메이션? 이밴트
	{
		caster.Now(life =>
		{
			if(life != null)
			{
				List<string> hitEffs = (self.atk as PlayerAttack).onNextHits?.Invoke(relatedTransform.gameObject, self, life.GetActor(), this);
				DoDamage(life.GetActor(), self);
				CameraManager.instance.ShakeCamFor(0.1f);
				//Debug.Log(hitEffs.Count);
				Vector3 effPos = life.transform.GetComponent<Collider>().ClosestPointOnBounds(caster.transform.position);
				if (hitEffs != null && hitEffs.Count > 0)
				{
					for (int i = 0; i < hitEffs.Count; i++)
					{
						PoolManager.GetObject(hitEffs[i], effPos, -caster.transform.forward, 2.5f);
					}

				}
				else
				{
					PoolManager.GetObject("Hit 26", effPos, -caster.transform.forward, 2.5f);

				}
			}
			
		});
	}
	
}
