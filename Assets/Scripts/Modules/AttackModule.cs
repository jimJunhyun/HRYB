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

	public virtual float prepMod { get;set;} = 1;

	public float atkGap;

	public AttackStates attackState
	{
		get;
		protected set;
	} = AttackStates.None;

	public virtual void Attack()
	{

	}
}
