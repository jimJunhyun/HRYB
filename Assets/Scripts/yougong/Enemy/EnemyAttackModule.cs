using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAttackModule : AttackModule
{
	Actor _target;

	public Actor Target => _target;

	
	protected string AttackStd;
	public override void Attack() { }

	public virtual void SetTarget(Actor a)
	{
		_target = a;
	}

	public void SetAttackType(string AttackName)
	{
		AttackStd = AttackName;
	}

	public abstract void SetAttackRange(int idx);
	public abstract void ResetAttackRange(int idx);

	public abstract void OnAnimationStop();
	public abstract void OnAnimationEvent();
	public abstract void OnAnimationEnd();


}
