using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AttackModule : Module, IAnimationEvent
{
	public float initAtkDist;
	public float initAtkGap;
	public YinYang initDamage;

	protected YinYang damage;

	protected float atkDist;

	protected float curPrepMod = 1;

	protected bool notattack = false;
	public bool NotAttack
	{
		get => notattack;
		set => notattack = value;
	}

	public virtual float prepMod
	{
		get
		{
			return fixedPrepMod == null ? curPrepMod : (float)fixedPrepMod;
		}
		set
		{
			curPrepMod = value;
		}
	}
	public float? fixedPrepMod = null;

	protected float curAtkGap;
	public float? fixedAtkGap = null;
	public float atkGap
	{
		get => fixedAtkGap == null ? curAtkGap : (float)fixedAtkGap;
		set => curAtkGap = value;
	}

	public float GetDist()
	{
		return atkDist;
	}

	public virtual void Attack()
	{

	}
	public override void ResetStatus()
	{
		base.ResetStatus();
		atkDist = initAtkDist;
		curPrepMod = 1;
		curAtkGap = initAtkGap;
		fixedPrepMod = null;
		fixedAtkGap = null;
		damage = initDamage;
	}

	public virtual void SetAttackRange(int idx)
	{
		// SetRange
	}

	public virtual void ResetAttackRange(int idx)
	{
		// ResetRange
	}

	public virtual void OnAnimationStart()
	{
		//
	}

	public virtual void OnAnimationMove()
	{
		//
	}

	public virtual void OnAnimationEvent()
	{
		// AnimEvent == SetRange
	}

	public virtual void OnAnimationStop()
	{
		
	}

	public virtual void OnAnimationEnd()
	{
		// AnimEnd == ResetRange
	}
}
