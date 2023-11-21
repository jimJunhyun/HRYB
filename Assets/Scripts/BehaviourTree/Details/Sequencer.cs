using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequencer : INode
{
	public List<INode> connecteds = new List<INode>();
	public NodeStatus Examine()
	{
		for (int i = 0; i < connecteds.Count; i++)
		{
			switch (connecteds[i].Examine())
			{
				case NodeStatus.Run:
					//Debug.Log("Seq running a " + i);
					return NodeStatus.Run;
				case NodeStatus.Sucs:
					//Debug.Log("Seq succ, next a " + i);
					break;
				case NodeStatus.Fail://Debug.Log("Seq faioled a " + i);
					return NodeStatus.Fail; 
				default:
					break;
			}
		}
		return NodeStatus.Sucs;
	}
}
