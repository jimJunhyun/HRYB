using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;




public abstract class Compose : ScriptableObject, IComposer, IAnimationEventActor
{
	public AnimationClip animClip;
	public AnimationClip animClipDisop;
	public string audioClipName;

	public SkillTag tags;

	public int level;

	//public UnityEvent<Compose> onOperateSelf;

	public abstract void Disoperate(Actor self);

	public abstract void Operate(Actor self);


	internal abstract void MyOperation(Actor self);
	internal abstract void MyDisoperation(Actor self);
	public abstract void UpdateStatus();
	
	
	public virtual void OnAnimationStart(Actor self)
	{
		
	}

	public virtual void OnAnimationMove(Actor self)
	{

	}

	public virtual void OnAnimationEvent(Actor self)
	{

	}

	public virtual void OnAnimationStop(Actor self)
	{

	}

	public virtual void OnAnimationEnd(Actor self)
	{

	}
}
