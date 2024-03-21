using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;




public abstract class Compose : ScriptableObject, IComposer, IAnimationEventActor
{
	public AnimationClip animClip;
	public AnimationClip animClipLoopings;
	public AnimationClip animClipDisop;
	public string audioClipName;

	[Tooltip("2진법 스타일.\n공격 : 1\n강화 : 2\n강화가능 : 4\n특수 : 8\n")]
	public SkillTag tags;

	public int level;

	//public UnityEvent<Compose> onOperateSelf;

	public abstract void Disoperate(Actor self);

	public abstract void Operate(Actor self);


	internal abstract void MyOperation(Actor self);
	internal abstract void MyDisoperation(Actor self);
	public abstract void UpdateStatus();


	public virtual void OnAnimationStart(Actor self, AnimationEvent evt){}

	public virtual void OnAnimationMove(Actor self, AnimationEvent evt){}
	public virtual void OnAnimationEvent(Actor self, AnimationEvent evt){}

	public virtual void OnAnimationStop(Actor self, AnimationEvent evt){}

	public virtual void OnAnimationEnd(Actor self, AnimationEvent evt){}
	
}
