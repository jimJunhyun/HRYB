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

	protected virtual YinYang DoDamage(Actor to, Actor by)
	{
		YinYang dmg = by.atk.initDamage * damageMult;
		to.life.DamageYY(by.atk.initDamage * damageMult, DamageType.DirectHit, 0, 0, by);
		Debug.Log($"[데미지] {to.gameObject.name} 에게 데미지 : {by.atk.initDamage} * {damageMult} = {(by.atk.initDamage * damageMult)}");
		for (int i = 0; i < statEff.Count; i++)
		{
			StatusEffects.ApplyStat(to, by, statEff[i].id, statEff[i].duration, statEff[i].power);
			
		}
		return dmg;
	}
}
