using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsUnderBalance : INode
{
    Actor self;
	float standard;

	public IsUnderBalance(Actor self, float strd)
	{
		this.self = self;
		standard = strd;
	}
	public NodeStatus Examine()
	{
		if(self.life.yy.white > standard)
		{
			return NodeStatus.Fail;
		}
		return NodeStatus.Sucs;
	}
}
