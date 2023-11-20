using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inverter : INode
{
	public INode connected;

	public NodeStatus Examine()
	{
		switch (connected.Examine())
		{
			case NodeStatus.Run:
				return NodeStatus.Run;
			case NodeStatus.Sucs:
				return NodeStatus.Fail;
			case NodeStatus.Fail:
				return NodeStatus.Sucs;
		}
		return NodeStatus.Fail;
	}
}
