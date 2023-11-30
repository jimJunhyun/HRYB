using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public abstract class Compose : ScriptableObject, IComposer
{
	public string animVarName;

	public abstract void Disoperate(Actor self);

	public abstract void Operate(Actor self);


	protected abstract void MyOperation(Actor self);
	protected abstract void MyDisoperation(Actor self);
}
