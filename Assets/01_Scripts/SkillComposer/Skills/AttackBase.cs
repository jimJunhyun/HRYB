using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackBase : Leaf
{
	public YinYang damage;
	public string relateTrmName;
	public List<StatusEffectApplyData> statEff;

	protected Transform relatedTransform;

	protected virtual void OnValidate()
	{
		relatedTransform = GameObject.Find(relateTrmName).transform;
		if(relatedTransform == null)
		{
			Debug.Log($"NO TRANSFORM FOUND IN SUCH NAME : {relateTrmName}");
		}
	}

	protected void DoDamage(Actor to, Actor by)
	{
		to.life.DamageYY(damage, DamageType.DirectHit);
		for (int i = 0; i < statEff.Count; i++)
		{
			StatusEffects.ApplyStat(to, by, statEff[i].id, statEff[i].duration, statEff[i].power);
			
		}
	}
}
