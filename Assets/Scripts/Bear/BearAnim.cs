using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearAnim : AnimModule
{
	protected readonly int attackAltHash = Animator.StringToHash("AttackAlt");
	protected readonly int attack2Hash = Animator.StringToHash("Attack2");
	protected readonly int attack3Hash = Animator.StringToHash("Attack3");
	protected readonly int sleepHash = Animator.StringToHash("Sleep");
	int atkCnt = 0;
	public override void SetAttackTrigger()
	{
		switch((GetActor().atk as BearAttack).nextAttackCall)
		{
			case AttackType.HandAttack:
				atkCnt += 1;
				if(atkCnt % 2 == 0)
				{
					anim.SetTrigger(attackHash);
				}
				else
				{
					anim.SetTrigger(attackAltHash);
				}
				break;
			case AttackType.MouthAttack:
				anim.SetTrigger(attack2Hash);
				break;
			case AttackType.SpecialAttack:
				anim.SetTrigger(attack3Hash);
				break;
		}
	}

	public void ResetSleepMode()
	{
		anim.SetBool(sleepHash, false);
	}
}
