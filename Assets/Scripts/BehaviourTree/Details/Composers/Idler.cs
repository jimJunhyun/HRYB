using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idler : INode
{
	Actor self;

	public Idler(Actor self)
	{
		this.self = self;
	}


	public NodeStatus Examine()
	{
		self.anim.SetIdleState(true);
		return NodeStatus.Run;
	}
}
