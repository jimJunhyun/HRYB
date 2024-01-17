using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public enum SkillTags
{
	All = -1,
	None = 0,

	AttackEnhance = 1,
	NonAttackEnhance = 2,
	AttackEnhancable = 4,
	NonAttackEnhancable = 8,


}


public abstract class Compose : ScriptableObject, IComposer
{
	public AnimationClip animClip;
	public AnimationClip animClipDisop;
	public string audioClipName;

	public virtual int tags
	{
		get;
		set;
	}

	public bool ContainsTag(params SkillTags[] objs)
	{
		bool res = true;
		for (int i = 0; i < objs.Length; i++)
		{
			int digit = (int)Mathf.Log(((int)objs[i]), 2);
			res &= (tags >> digit) % 2 == 1;
		}
		return res;
	}

	//public UnityEvent<Compose> onOperateSelf;

	public abstract void Disoperate(Actor self);

	public abstract void Operate(Actor self);


	internal abstract void MyOperation(Actor self);
	internal abstract void MyDisoperation(Actor self);
	public abstract void UpdateStatus();
}
