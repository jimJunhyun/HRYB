using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackStates
{
	None,
	Prepare,
	Trigger,

}

public class AttackModule : Module
{
	public YinyangWuXing damage;
	public float effSpeed;
	public float effSpeedMod = 1f;
	public float EffSpeed { get => effSpeed * effSpeedMod;}
	public bool isDirect;

	public float atkDist;

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
}
