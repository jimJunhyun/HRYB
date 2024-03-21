using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAttackModule : AttackModule
{
	Actor _target;
	protected ColliderCast _nowCols;
	public Actor Target => _target;


	public void ResetCols()
	{
		if (_nowCols != null)
		{
			_nowCols.End();
			_nowCols = null;
		}
	}
	
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

	public abstract void OnAnimationStart();
	public abstract void OnAnimationMove();

	public abstract void OnAnimationSound();
	public abstract void OnAnimationStop();
	public abstract void OnAnimationEvent();
	public abstract void OnAnimationEnd();


}
