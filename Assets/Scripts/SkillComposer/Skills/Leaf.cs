using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class  Leaf : Compose
{
	//public UnityAction act;

	private int? myVarHash = null;

	public override void Operate(Actor self)
	{
		if(myVarHash == null)
			myVarHash = Animator.StringToHash(animVarName);
		self.anim.SetTrigger((int)myVarHash);
		MyOperation(self);
	}

	public override void Disoperate(Actor self)
	{
		MyDisoperation(self);
	}

	protected abstract override void MyOperation(Actor self);
	protected abstract override void MyDisoperation(Actor self);
}
