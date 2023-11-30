using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAttackModule : AttackModule
{
	Actor _target;

	public Actor Target => _target;
	
	protected string AttackStd;
	public abstract void Attack();
	
	public virtual void SetTarget(Actor a)
	{
		_target = a;
	}

	public void SetAttackType(string atd)
	{
		AttackStd = atd;
	}

	public abstract void SetAttackRange(int idx);
	public abstract void ResetAttackRange(int idx);
	public abstract void OnAnimationEvent();
	public abstract void OnAnimationEnd();
}
