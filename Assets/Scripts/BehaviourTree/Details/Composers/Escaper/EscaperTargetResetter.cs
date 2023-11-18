using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscaperTargetResetter : INode
{
	Actor self;
	public EscaperTargetResetter(Actor self)
	{
		this.self = self;
	}
	public NodeStatus Examine()
	{
		(self.move as EscaperMove).SetTarget(null);
		self.anim.SetMoveState(0);
		return NodeStatus.Run;
	}
}
