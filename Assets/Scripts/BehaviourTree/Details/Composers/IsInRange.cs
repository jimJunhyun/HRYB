using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsInRange : INode
{
	public float rng;
	Actor self;
	Transform target;

	public IsInRange(Actor self, Transform target, float dist)
	{
		this.self = self;
		this.target = target;
		rng = dist;
	}

	public NodeStatus Examine()
	{
		Vector3 dir = (self.transform.position - target.position);
		if (dir.sqrMagnitude > rng * rng)
		{
			return NodeStatus.Fail;
		}
		else
		{
			return NodeStatus.Sucs;
		}
	}
}
