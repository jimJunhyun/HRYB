using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;




public abstract class Compose : ScriptableObject, IComposer
{
	public AnimationClip animClip;
	public AnimationClip animClipDisop;
	public string audioClipName;

	//public UnityEvent<Compose> onOperateSelf;

	public abstract void Disoperate(Actor self);

	public abstract void Operate(Actor self);


	internal abstract void MyOperation(Actor self);
	internal abstract void MyDisoperation(Actor self);
	public abstract void UpdateStatus();
}
