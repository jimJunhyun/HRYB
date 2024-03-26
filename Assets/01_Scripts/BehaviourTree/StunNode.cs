using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunNode : INode
{
	Actor self;
	System.Action wakeUp;
	private bool F = false;
	
	public StunNode(Actor self, System.Action state = null)
	{
		F = false;
		wakeUp = state;	
		this.self = self;
	}

    public NodeStatus Examine()
    {
	    if (StatusEffects._bufferDurations.ContainsKey(StatEffID.Stun))
	    {
		    if (StatusEffects._bufferDurations[StatEffID.Stun] >= 0)
		    {
				EnemyMoveModule moves = self.move as EnemyMoveModule;
				moves.StopMove();
			    self.anim.SetBoolModify("Stun", true);
			    if (F == false || self.anim.Animators.GetBool("Stun") == false)
			    {
				    EnemyAttackModule atk  = self.atk as EnemyAttackModule;
				    atk.ResetCols();
					F = true;
			    }
			    
			    return NodeStatus.Run;
		    }
	    }
	    if (F)
	    {
		    self.anim.SetBoolModify("Stun", false);
		    F = false;
		    wakeUp?.Invoke();
	    }

	    return NodeStatus.Fail;
    }
}
