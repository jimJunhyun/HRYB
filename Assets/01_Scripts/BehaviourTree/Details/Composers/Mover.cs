using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : INode
{
	Actor self;
	Action t;

	public Mover(Actor self, Action f = null)
	{
		this.self = self;
		t = f;
	}

	public NodeStatus Examine()
	{
		self.move.Move();
		t?.Invoke();
		return NodeStatus.Run;
	}
}
