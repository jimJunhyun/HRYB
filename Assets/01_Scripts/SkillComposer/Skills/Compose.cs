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

	[Tooltip("2진법 스타일.\n공격강화 : 1\n비공격강화 : 2\n강화가능공격 : 4\n강화가능비공격 : 8\n강화불가공격 : 16\n강화불가비공격 : 32\n특수 : 64\n")]
	public SkillTag tags;

	public int level;

	//public UnityEvent<Compose> onOperateSelf;

	public abstract void Disoperate(Actor self);

	public abstract void Operate(Actor self);


	internal abstract void MyOperation(Actor self);
	internal abstract void MyDisoperation(Actor self);
	public abstract void UpdateStatus();
}
