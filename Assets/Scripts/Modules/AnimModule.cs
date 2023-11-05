using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimModule : Module
{
	protected readonly int moveHash = Animator.StringToHash("Move");
	protected readonly int idleHash = Animator.StringToHash("Idle");
	protected readonly int jumpHash = Animator.StringToHash("Jump");
	protected readonly int attackHash = Animator.StringToHash("Attack");
	protected readonly int moveXHash = Animator.StringToHash("MoveX");
	protected readonly int moveYHash = Animator.StringToHash("MoveY");


	protected Animator anim;

	public virtual void Awake()
	{
		anim = GetComponent<Animator>();
	}


	public virtual void SetAttackTrigger()
	{
		anim.SetTrigger(attackHash);
	}
	
}
