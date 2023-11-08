using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsInRange : INode
{
	public float rng;
	Actor self;
	Transform target;
	System.Action foundAction;

	public IsInRange(Actor self, Transform target, float dist, System.Action onFound = null)
	{
		this.self = self;
		this.target = target;
		rng = dist;
		foundAction = onFound;
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
			foundAction?.Invoke();
			return NodeStatus.Sucs;
		}
	}
}
