using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JangSungAI : AISetter
{
	private const string JumpAttackHash = "JumpAttack";
	private readonly int _isJumpAttack = Animator.StringToHash(JumpAttackHash);
	
	private const string FallDownAttackHash = "FallDownAttack";
	private readonly int _isFallDownAttack = Animator.StringToHash(FallDownAttackHash);

	private const string MoveAttackHash = "MoveAttack";
	private readonly int _isMoveAttack = Animator.StringToHash(MoveAttackHash);
	
    protected override void StartInvoke()
    {
	    
    }

    protected override void UpdateInvoke()
    {
	    
    }
}
