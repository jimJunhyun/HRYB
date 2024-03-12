using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Leaf : Compose
{
	//public UnityAction act;

	private int? myVarHash = null;

	public override void Operate(Actor self)
	{
		MyOperation(self);
	}

	public override void Disoperate(Actor self)
	{
		MyDisoperation(self);
	}

	internal abstract override void MyOperation(Actor self);
	internal abstract override void MyDisoperation(Actor self);
}
