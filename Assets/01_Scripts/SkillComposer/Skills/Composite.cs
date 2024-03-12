using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Skills/Composite")]

public class Composite : Compose, IComposer, IAnimationEventActor
{
	public List<Compose> childs;
	public float composeDel;
	public bool isPlayAnim = false;
	public bool isPlayDisopAnim = false;

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
		Debug.Log("OPERATING : " + name);
		GameManager.instance.StartCoroutine(DelOperate(self));
	}

	public void OperateAt(Actor self, int idx)
	{
		childs[idx].Operate(self);
	}

	public void ActualOperateAt(Actor self, int idx)
	{
		childs[idx].MyOperation(self);
	}

	public void DisoperateAt(Actor self, int idx)
	{
		childs[idx].Disoperate(self);
	}

	public void ActualDisoperateAt(Actor self, int idx)
	{
		childs[idx].MyDisoperation(self);
	}

	internal override void MyDisoperation(Actor self)
	{
		//Do nothing?
	}

	internal override void  MyOperation(Actor self)
	{
		for (int i = 0; i < childs.Count; i++)
		{
			childs[i].MyOperation(self);
		}
	}

	public override void UpdateStatus()
	{
		for (int i = 0; i < childs.Count; i++)
		{
			childs[i].UpdateStatus();
		}
	}

	public virtual void SetAnimations(Actor to, SkillSlotInfo info)
	{
		if((to.anim as PlayerAnim).curEquipped != this)
		{
			List<AnimationClip> atoms = new List<AnimationClip>();
			for (int i = 0; i < childs.Count; i++)
			{
				AnimationClip clip;
				clip = childs[i].animClip;
				Debug.Log("??????????");
				if(clip != null)
				{
					AnimationEvent[] events = clip.events;

					events[1].intParameter = i;
					events[1].stringParameter = info.ToString();
					events[2].intParameter = i;
					events[2].stringParameter = info.ToString();
					clip.events = events;
					
					atoms.Add(clip);
					Debug.Log($"{clip.name} : {clip.events[1].intParameter}-{clip.events[1].stringParameter}, {events[2].intParameter}-{clip.events[2].stringParameter}");
				}
			}
			to.anim.SetAnimationOverrides(new List<string>() { "SkillAtom0", "SkillAtom1", "SkillAtom2", "SkillAtom3", "SkillAtom4" }, atoms);
			(to.anim as PlayerAnim).curEquipped = this;
		}
	}

	IEnumerator DelOperate(Actor self)
	{
		for (int i = 0; i < childs.Count; i++)
		{
			Debug.Log("ARROW SHOOT : " + childs[i].name);
			if (isPlayAnim)
			{
				(GameManager.instance.pActor.anim as PlayerAnim).SetSkillAtomCount(childs.Count - 1);
				(GameManager.instance.pActor.anim as PlayerAnim).SetAttackTrigger();
			}
			else
			{
				childs[i].Operate(self);
			}
			yield return new WaitForSeconds(composeDel);
		}
		//onOperateSelf = null;
	}

	IEnumerator DelDisoperate(Actor self)
	{
		for (int i = 0; i < childs.Count; i++)
		{
			childs[i].Disoperate(self);
			if (isPlayDisopAnim)
			{
				(GameManager.instance.pActor.anim as PlayerAnim).SetDisopTrigger(i);
			}
			yield return new WaitForSeconds(composeDel);
		}
	}
	
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
