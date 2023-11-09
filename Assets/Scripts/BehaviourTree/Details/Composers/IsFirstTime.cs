using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsFirstTime : INode
{
	bool first = true;

	public NodeStatus Examine()
	{
		if (first)
		{
			return NodeStatus.Sucs;
		}
		return NodeStatus.Fail;
	}
}
