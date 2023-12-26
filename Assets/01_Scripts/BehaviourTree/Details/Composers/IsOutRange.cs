using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsOutRange : INode
{
	Func<float> range;
	Func<float> sneakDecFunc;
	Actor self;
	Transform target;
	System.Action foundAction;

	public IsOutRange(Actor self, Transform target, Func<float> dist,  Func<float> distDec, Action onFound = null)
	{
		this.self = self;
		this.target = target;
		range = dist;
		sneakDecFunc = distDec;
		foundAction = onFound;
	}

	public NodeStatus Examine()
	{
		Vector3 dir = (self.transform.position - target.position);
		float sqrRng = sneakDecFunc == null ? range() * range() : (range() - sneakDecFunc()) * (range() - sneakDecFunc());
		//Debug.Log("EXAMINING" + target.name);
		if (dir.sqrMagnitude < sqrRng)
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
