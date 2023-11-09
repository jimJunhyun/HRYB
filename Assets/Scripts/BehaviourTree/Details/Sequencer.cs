using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequencer : INode
{
	public List<INode> connecteds = new List<INode>();
	public NodeStatus Examine()
	{
		Debug.Log("Sequencer Exam");
		for (int i = 0; i < connecteds.Count; i++)
		{
			switch (connecteds[i].Examine())
			{
				case NodeStatus.Run:
					return NodeStatus.Run;
				case NodeStatus.Sucs:
					break;
				case NodeStatus.Fail:
					return NodeStatus.Fail;
				default:
					break;
			}
		}
		return NodeStatus.Sucs;
	}
}
