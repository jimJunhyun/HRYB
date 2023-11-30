using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AttackModule : Module
{
	public float initAtkDist;
	public float initAtkGap;
	public YinYang initDamage;

	protected YinYang damage;

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
		atkDist = initAtkDist;
		curPrepMod = 1;
		curAtkGap = initAtkGap;
		fixedPrepMod = null;
		fixedAtkGap = null;
		damage = initDamage;
	}
}
