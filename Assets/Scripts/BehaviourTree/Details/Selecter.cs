using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selecter : INode
{
	public List<INode> connecteds = new List<INode>();
	public NodeStatus Examine()
	{
		Debug.Log("Selector Exam");
		for (int i = 0; i < connecteds.Count; i++)
		{
			switch (connecteds[i].Examine())
			{
				case NodeStatus.Run:
					return NodeStatus.Run;
				case NodeStatus.Fail:
					break;
				case NodeStatus.Sucs:
					return NodeStatus.Sucs;
				default:
					break;
			}
		}
		return NodeStatus.Fail;
	}
}
