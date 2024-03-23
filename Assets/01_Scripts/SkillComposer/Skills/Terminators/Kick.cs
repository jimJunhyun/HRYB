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
		Debug.Log("!@!@!@!@!@!@");
		(self.atk as PlayerAttack).onNextUse?.Invoke(relatedTransform.gameObject);
	}

	public override void UpdateStatus()	// ?? 스킬 시전중
	{
		
	}

	internal override void MyDisoperation(Actor self) // 사라질 때
	{
		caster.End();
		//트레일지워주기
		Debug.Log("KICKENDER");
	}

	internal override void MyOperation(Actor self) // 애니메이션? 이밴트
	{
		caster.Now( self.transform,life =>
		{
			if(life != null)
			{
				
				DoDamage(life.GetActor(), self);
				CameraManager.instance.ShakeCamFor(0.1f);
				//Debug.Log(hitEffs.Count);
				Vector3 effPos = life.transform.GetComponent<Collider>().ClosestPointOnBounds(caster.transform.position);
				if ((self.atk.Damage * damageMult).white > 0)
				{
					GameManager.instance.shower.GenerateDamageText(effPos, (self.atk.Damage * damageMult).white, YYInfo.White);
				}
				if ((self.atk.Damage * damageMult).black > 0)
				{
					GameManager.instance.shower.GenerateDamageText(effPos, (self.atk.Damage * damageMult).black, YYInfo.Black);
				}
				(self.atk as PlayerAttack).onNextSkill?.Invoke(self, this);
				(self.atk as PlayerAttack).onNextHit?.Invoke(effPos);
				PoolManager.GetObject("Hit 26", effPos, -caster.transform.forward, 2.5f);
			}
			
		});
	}
	
}
