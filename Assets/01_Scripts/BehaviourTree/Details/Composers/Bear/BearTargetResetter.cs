using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearTargetResetter : INode
{
	Actor self;
	public BearTargetResetter(Actor self)
	{
		this.self = self;
	}
	public NodeStatus Examine()
	{
		(self.move as BearMove).SetTarget(null);
		GameManager.instance.sManager.RevertState();
		self.anim.SetMoveState(0);
		return NodeStatus.Run;
	}
}
