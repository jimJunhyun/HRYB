using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
	HandAttack,

	MouthAttack = 2,
	SpecialAttack,
}


public class BearAttack : AttackModule
{
	public float atkDist2;
	public float atkDist3;


	List<GameObject> ranges;

	public AttackType nextAttackCall = AttackType.HandAttack;

	Actor target;

	private void Awake()
	{
		ranges.Add(transform.Find("Range1").gameObject);
		ranges.Add(transform.Find("Range2").gameObject);
		ResetAttackRange(0);
		ResetAttackRange(1);
	}

	public void SetTarget(Actor a)
	{
		target = a;
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
				target.life.AddYYWX(damage, EffSpeed, true);
				break;
			case AttackType.SpecialAttack:
				//대지가르기 생성
				break;
		}
		GetActor().anim.SetAttackTrigger();
	}

	public void SetAttackRange(int idx)
	{
		ranges[idx].SetActive(true);
	}

	public void ResetAttackRange(int idx)
	{
		ranges[idx].SetActive(false);
	}

	
}
