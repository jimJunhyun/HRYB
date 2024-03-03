using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public enum AttackType
{
	HandAttack,

	MouthAttack = 2,
	SpecialAttack,
}


public class BearAttack : EnemyAttackModule
{
	public float atkDist2;
	public float atkDist3;

	public float GetDist2()
	{
		return atkDist2;
	}

	public float GetDist3()
	{
		return atkDist3;
	}


	List<GameObject> ranges= new List<GameObject>();
	PlayGroundBreak gb;
	AnimationEffectPlayer scratch;

	public AttackType nextAttackCall = AttackType.HandAttack;


	public void Awake()
	{
		ranges.Add(transform.Find("Range1").gameObject);
		ranges.Add(transform.Find("Range2").gameObject);
		gb = transform.Find("GroundBreak").GetComponent<PlayGroundBreak>();
		scratch = transform.Find("ScrEffect").GetComponent<AnimationEffectPlayer>();
		ResetAttackRange(0);
		ResetAttackRange(1);
	}


    public void SetAttackType(AttackType type)
	{
		nextAttackCall = type;
	}

	public override void Attack()
	{
		switch (nextAttackCall)
		{
			case AttackType.HandAttack:

				break;
			case AttackType.MouthAttack:
				Target.life.DamageYY(damage, DamageType.DirectHit);
				PoolManager.GetObject("DamagedEffect", Target.transform.position + (Vector3.up * Target.transform.localScale.magnitude * 0.5f), Quaternion.LookRotation(Target.transform.forward));
				break;
			case AttackType.SpecialAttack:
				
				break;
		}
		GetActor().anim.SetAttackTrigger();
	}

	public override void SetAttackRange(int idx)
	{
		ranges[idx].SetActive(true);
		if(idx == 0)
		{
			scratch.PlayEffect();
			scratch.Rotate180();
		}
		if(idx == 1)
		{
			gb.PlayEffect();
		}
	}

	public override void ResetAttackRange(int idx)
	{
		ranges[idx].SetActive(false);
	}

	public override void OnAnimationStart()
	{
		
	}

	public override void OnAnimationMove()
	{
		throw new System.NotImplementedException();
	}

	public override void OnAnimationSound()
	{
		throw new System.NotImplementedException();
	}


	public override void OnAnimationStop()
	{
		
	}

	public override void OnAnimationEvent() { }

	public override void OnAnimationEnd() { }
}
