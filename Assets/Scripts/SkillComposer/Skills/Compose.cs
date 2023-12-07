using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public abstract class Compose : ScriptableObject, IComposer
{
	public AnimationClip animClip;
	public string audioClipName;

	public abstract void Disoperate(Actor self);

	public abstract void Operate(Actor self);


	internal abstract void MyOperation(Actor self);
	internal abstract void MyDisoperation(Actor self);
	public abstract void UpdateStatus();
}
