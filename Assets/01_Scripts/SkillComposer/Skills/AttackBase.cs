using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackBase : Leaf
{
	public float damageMult;
	public string relateTrmName;
	

	protected Transform relatedTransform;

	protected virtual void OnValidate()
	{
		relatedTransform = GameObject.Find(relateTrmName)?.transform;
		if(relatedTransform == null)
		{
			Debug.Log($"NO TRANSFORM FOUND IN SUCH NAME : {relateTrmName}");
		}
		else
		{
			Debug.Log($"NO PROBLEM WITH {name}");
		}
	}

	public override void Operate(Actor self) // 실행시
	{

	}


	protected virtual void DoDamage(Actor to, Actor by)
	{
		
		float white = by.atk.Damage.white * damageMult;
		float black = by.atk.Damage.white * damageMult;
		
		to.life.DamageYY(black,white, DamageType.DirectHit, 0, 0, by);
		Debug.Log($"[데미지] {to.gameObject.name} 에게 데미지 : {by.atk.initDamage} * {damageMult} = {(by.atk.initDamage * damageMult)}");
		
		
		if ((by.atk.Damage * damageMult).white > 0)
		{
			GameManager.instance.shower.GenerateDamageText(to.transform.position, white, YYInfo.White);
		}
		if ((by.atk.Damage * damageMult).black > 0)
		{
			GameManager.instance.shower.GenerateDamageText(to.transform.position, black, YYInfo.Black);
		}
		
		for (int i = 0; i < statEff.Count; i++)
		{
			StatusEffects.ApplyStat(to, by, statEff[i].id, statEff[i].duration, statEff[i].power);
		}
	}
	
	
	protected virtual void DoDamage(Actor to, Actor by, float value)
	{
		if (value == 0)
			value = 1;

		float white = by.atk.Damage.white * damageMult * value;
		float black = by.atk.Damage.white * damageMult * value;
		
		if ((by.atk.Damage * damageMult).white > 0)
		{
			GameManager.instance.shower.GenerateDamageText(to.transform.position, white, YYInfo.White);
		}
		if ((by.atk.Damage * damageMult).black > 0)
		{
			GameManager.instance.shower.GenerateDamageText(to.transform.position,black, YYInfo.Black);
		}
		
		to.life.DamageYY(black, white, DamageType.DirectHit, 0, 0, by);
		for (int i = 0; i < statEff.Count; i++)
		{
			StatusEffects.ApplyStat(to, by, statEff[i].id, statEff[i].duration, statEff[i].power);
		}
	}
	
}
