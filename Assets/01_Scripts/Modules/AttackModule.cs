using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AttackModule : Module, IAnimationEvent
{
	public float initAtkDist;
	public float initAtkGap;
	public YinYang initDamage;

	protected YinYang damage;

	public YinYang Damage
	{
		get => damage;
	}

	protected float atkDist;


	public Transform target;

	public ModuleController attackModuleStat = new ModuleController(false);

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
		curAtkGap = initAtkGap;
		fixedAtkGap = null;
		damage = initDamage;
		attackModuleStat.CompleteReset();
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
