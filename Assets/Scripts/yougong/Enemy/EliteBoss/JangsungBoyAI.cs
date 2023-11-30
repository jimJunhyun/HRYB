using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JangsungBoyAI : AISetter
{
	public List<Actor> _friend = new();
	
	private const string JumpAttackHash = "JumpAttack";
	private readonly int _isJumpAttack = Animator.StringToHash("JumpAttack");
	
	private const string FallDownAttackHash = "FallDownAttack";
	private readonly int _isFallDownAttack = Animator.StringToHash("FallDownAttack");
	
	private const string MoveAttackHash = "MoveAttack";
	private readonly int _isMoveAttack = Animator.StringToHash("MoveAttack");
	
    protected override void StartInvoke()
    {
	    
	    JangsungManAttackModule _jsAttckModule = self.atk as JangsungManAttackModule;
	    
	    
	    IsFirstTime once = new IsFirstTime();
	    List<IsTargetDead> pairDead = new();
	    
	    for (int i = 0; i < _friend.Count; i++)
	    {
		    pairDead.Add(new IsTargetDead(_friend[i]));
	    }
	    Waiter waitJump = new Waiter(1f);
	    IsInRange isJumpAtk = new IsInRange(self, player.transform, _jsAttckModule.JumpDist, null, () =>
	    {
			_jsAttckModule.SetAttackType(JumpAttackHash);
			waitJump.StartReady();
	    });
	    Attacker JumpAttack = new Attacker(self, () =>
	    {
			self.move.LookAt(player.transform);
	    });


    }

    protected override void UpdateInvoke()
    {
	    
    }
}
