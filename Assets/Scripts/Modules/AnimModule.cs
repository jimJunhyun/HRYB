using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimModule : Module
{
	protected readonly int moveHash = Animator.StringToHash("Move");
	protected readonly int idleHash = Animator.StringToHash("Idle");
	
	protected readonly int attackHash = Animator.StringToHash("Attack");
	protected readonly int moveXHash = Animator.StringToHash("MoveX");
	protected readonly int moveYHash = Animator.StringToHash("MoveY");
	protected readonly int hitHash = Animator.StringToHash("Hit");
	protected readonly int dieHash = Animator.StringToHash("Die");
	protected readonly int respawnHash = Animator.StringToHash("Respawn");




	protected Animator anim;

	public virtual void Awake()
	{
		anim = GetComponent<Animator>();
	}


	public virtual void SetAttackTrigger()
	{
		anim.SetTrigger(attackHash);
	}

	public virtual void SetHitTrigger()
	{
		anim.SetTrigger(hitHash);
	}

	public virtual void SetDieTrigger()
	{
		anim.SetTrigger(dieHash);
	}

	public virtual void SetMoveState(int val = 0)
	{
		anim.SetInteger(moveHash, val);
	}

	public virtual void SetIdleState(bool val)
	{
		anim.SetBool(idleHash, val);
	}

	public override void ResetStatus()
	{
		base.ResetStatus();

		anim.SetTrigger(respawnHash);
	}

}
