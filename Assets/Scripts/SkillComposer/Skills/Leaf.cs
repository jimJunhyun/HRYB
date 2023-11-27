using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "New Skill Leaf")]
[System.Serializable]
public class Leaf : Compose
{
	//public UnityAction act;

	public override sealed void Operate()
	{
		MyOperation();
	}

	protected override void MyOperation()
	{
		
	}
}
