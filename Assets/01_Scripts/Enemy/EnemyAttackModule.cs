using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public abstract class EnemyAttackModule : AttackModule, IAnimationEventActorEv
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

	public void GiveBuff(Actor act, StatEffID id, float duration)
	{
		StatusEffects.ApplyStat(act, self, id, duration, 0);
	}
	
	public virtual void OnAnimationStart(AnimationEvent evt)
	{

	}
	public virtual void OnAnimationMove(AnimationEvent evt)
	{

	}

	public virtual void OnAnimationSound(AnimationEvent evt)
	{

	}
	public virtual void OnAnimationStop(AnimationEvent evt)
	{

	}
	public virtual void OnAnimationEvent(AnimationEvent evt)
	{

	}
	public virtual void OnAnimationEnd(AnimationEvent evt)
	{
		if (_nowCols != null)
		{
			_nowCols.End();
			_nowCols = null;
		}
	}

	public virtual void OnAnimationHit(AnimationEvent evt)
	{

	}
}
