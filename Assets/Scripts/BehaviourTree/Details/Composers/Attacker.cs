using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacker : INode
{
    Actor self;
	public Attacker(Actor self)
	{
		this.self = self;
	}

	public NodeStatus Examine()
	{
		self.atk.Attack();

		return NodeStatus.Run;
	}
}
