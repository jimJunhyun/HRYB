using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacker : INode
{
    Actor self;
	System.Action onAttack;
	public Attacker(Actor self, System.Action atk = null)
	{
		onAttack = atk;	
		this.self = self;
	}

	public NodeStatus Examine()
	{
		Debug.Log("Attacked");
		onAttack.Invoke();
		self.atk.Attack();

		return NodeStatus.Run;
	}
}
