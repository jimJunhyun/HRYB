using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsTargetDead : INode
{
	private Actor _act;

	public IsTargetDead(Actor act)
	{
		_act = act;
	}
	
	
    public NodeStatus Examine()
    {
	    if(_act.life.isDead == false)
	    {
		    return NodeStatus.Fail;
	    }
	    return NodeStatus.Sucs;
    }
}
