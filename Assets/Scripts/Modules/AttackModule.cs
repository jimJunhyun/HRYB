using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackStates
{
	None,
	Prepare,
	Trigger,

}

public class AttackModule : Module, IAnimationEvent
{
	public float initEffSpeed;
	public bool initIsDirect;
	public float initAtkDist;
	public float initAtkGap;
	public YinYang initDamage;

	protected YinYang damage;
	protected float effSpeed;
	public float effSpeedMod = 1f;
	public float EffSpeed { get => effSpeed * effSpeedMod;}
	public bool isDirect;

	protected float atkDist;

	protected float curPrepMod = 1;

	
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

	public AttackStates attackState
	{
		get;
		protected set;
	} = AttackStates.None;

	public virtual void Attack()
	{

	}
	public override void ResetStatus()
	{
		base.ResetStatus();
		effSpeed = initEffSpeed;
		isDirect = initIsDirect;
		atkDist = initAtkDist;
		curPrepMod = 1;
		effSpeedMod = 1;
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
