using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : INode
{
	Actor self;

	public Mover(Actor self)
	{
		this.self = self;
	}

	public NodeStatus Examine()
	{
		self.move.Move();
		return NodeStatus.Run;
	}
}
