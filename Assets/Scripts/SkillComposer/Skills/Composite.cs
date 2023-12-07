using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Skills/Composite")]

public class Composite : Compose, IComposer
{
	public List<Compose> childs;
	public float composeDel;
	public bool isPlayAnim = false;

	public void AddChild(Compose comp)
	{
		childs.Add(comp);
	}

	public override void Disoperate(Actor self)
	{
		GameManager.instance.StartCoroutine(DelDisoperate(self));
	}

	public override void Operate(Actor self)
	{
		GameManager.instance.StartCoroutine(DelOperate(self));
	}

	internal override void MyDisoperation(Actor self)
	{
		//Do nothing?
	}

	internal override void MyOperation(Actor self)
	{
		//Do nothing?
	}

	public override void UpdateStatus()
	{
		for (int i = 0; i < childs.Count; i++)
		{
			childs[i].UpdateStatus();
		}
	}

	public void SetAnimations(Actor to)
	{
		if((to.anim as PlayerAnim).curEquipped != this)
		{
			List<AnimationClip> clips = new List<AnimationClip>();
			for (int i = 0; i < childs.Count; i++)
			{
				clips.Add(childs[i].animClip);
				Debug.Log($"New Clip : {childs[i].animClip}");
			}
			to.anim.SetAnimationOverrides(new List<string>() { "Zero", "One", "Two", "Three", "Four" }, clips);
			(to.anim as PlayerAnim).curEquipped = this;
		}
		
	}

	IEnumerator DelOperate(Actor self)
	{
		for (int i = 0; i < childs.Count; i++)
		{
			childs[i].Operate(self);
			if (isPlayAnim)
			{
				(GameManager.instance.pActor.anim as PlayerAnim).SetAttackTrigger(i);
			}
			yield return new WaitForSeconds(composeDel);
		}
	}

	IEnumerator DelDisoperate(Actor self)
	{
		for (int i = 0; i < childs.Count; i++)
		{
			childs[i].Disoperate(self);
			yield return new WaitForSeconds(composeDel);
		}
	}
}
