using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
	HandAttack,
	MouthAttack,
	SpecialAttack,
}


public class BearAttack : AttackModule
{

	public YinyangWuXing damage2;

	public YinyangWuXing damage3;

	GameObject range1;

	AttackType nextAttackCall = AttackType.HandAttack;

	Actor target;

	private void Awake()
	{
		range1 = transform.Find("Range1").gameObject;
		ResetAttackRangeOne();
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
				GetActor().anim.SetAttackTrigger();
				break;
			case AttackType.MouthAttack:
				if (!target.life.isImmune)
				{
					target.life.AddYYWX(damage2, EffSpeed);
				}
				break;
			case AttackType.SpecialAttack:
				break;
		}
	}

	public void SetAttackRangeOne()
	{
		range1.SetActive(true);
	}

	public void ResetAttackRangeOne()
	{
		range1.SetActive(false);
	}
}
