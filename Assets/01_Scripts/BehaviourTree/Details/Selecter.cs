using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selecter : INode
{
	public List<INode> connecteds = new List<INode>();
	public NodeStatus Examine()
	{
		for (int i = 0; i < connecteds.Count; i++)
		{
			
			switch (connecteds[i].Examine())
			{
				case NodeStatus.Run:
					//Debug.Log("Running at " + i);
					return NodeStatus.Run;
				case NodeStatus.Fail:
					//Debug.Log("Fail, Next at " + i);
					break;
				case NodeStatus.Sucs:
					//Debug.Log("Sucssedded at " + i);
					return NodeStatus.Sucs;
				default:
					break;
			}
		}
		return NodeStatus.Fail;
	}
}
