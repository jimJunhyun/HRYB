using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Skills/Combo")]
public class ComboRoot : SkillRoot
{
	public int initCombo;
	public float resetSec;
	public int resetThreshold;
	

	int curCombo = 0;
	float prevOperateSec;

	public override void Disoperate(Actor self)
	{
		base.Disoperate(self);
	}

	public override void Operate(Actor self)
	{
		if(Time.time - prevOperateSec >= composeDel)
		{
			if (isSuperArmor)
			{
				self.life.superArmor = true;
			}
			if (self.anim is PlayerAnim pa)
			{
				pa.SetAttackTrigger(curCombo); 
				prevOperateSec = Time.time;
			}
		}
	}

	public override void UpdateStatus()
	{
		if(curCombo	> resetThreshold && Time.time - prevOperateSec >= resetSec && curCombo != 0)
		{
			Debug.Log("콤보유지시간초과");
			ResetCombo();
			prevOperateSec = Time.time;
		}
		base.UpdateStatus();
	}

	public int GetCombo()
	{
		return curCombo;
	}

	internal override void MyDisoperation(Actor self)
	{
		//Do nothing
	}

	internal override void MyOperation(Actor self)
	{
		if (curCombo >= childs.Count)
		{
			ResetCombo();
			Debug.Log("콤보 최대, 초기화");
		}
		Debug.Log($"콤보 {curCombo + 1}/{childs.Count}");

		//childs[curCombo].onOperateSelf += onOperateSelf;
		childs[curCombo].MyOperation(self);
		

		NextCombo(true, self);
	}

	public override void OnAnimationStart(Actor self, AnimationEvent evt)
	{
		if (curCombo >= childs.Count)
		{
			ResetCombo();
			Debug.Log("콤보 최대, 초기화");
		}
		Debug.Log($"콤보 {curCombo + 1}/{childs.Count}");
		
		//childs[curCombo].MyOperation(self);
		childs[evt.intParameter].OnAnimationStart(self, evt);
		

		NextCombo(true, self);
		
	}
	

	public override void SetAnimations(Actor to, SkillSlotInfo info)
	{
		if ((to.anim as PlayerAnim).curEquipped != this)
		{
			List<AnimationClip> clips = new List<AnimationClip>();
			for (int i = 0; i < childs.Count; i++)
			{
				if (childs[i].animClip != null)
				{
					AnimationClip clip = childs[i].animClip;
					AnimationEvent[] events = clip.events;
					if(events.Length > 0)
					{
						for (int j = 0; j < events.Length; j++)
						{
							string t = events[j].stringParameter.Split("$")[0];
							
							events[j].stringParameter = t + "$" + info.ToString();
							events[j].intParameter = i;
						}
						clip.events = events;
						Debug.Log($"{clip.name} : {clip.events[0].intParameter}-{clip.events[0].stringParameter}");
					}
					
					clips.Add(clip);
				}
			}
			for (int i = 0; i < 5 - childs.Count; i++)
			{
				clips.Add(null);
			}

			for (int i = 0; i < childs.Count; i++)
			{
				if (childs[i].animClipDisop != null)
				{
					AnimationClip clip = childs[i].animClipDisop;
					AnimationEvent[] events = clip.events;
					if(events.Length > 0)
					{
						for (int j = 0; j < events.Length; j++)
						{
							events[j].stringParameter = info.ToString();
							events[j].intParameter = i;
						}
						clip.events = events;
						Debug.Log($"{clip.name} : {clip.events[0].intParameter}-{clip.events[0].stringParameter}");
					}
					
					clips.Add(clip);
				}

			}
			for (int i = 0; i < 5 - childs.Count; i++)
			{
				clips.Add(null);
			}
			//dur
			for (int i = 0; i < childs.Count; i++)
			{
				if (childs[i].animClipLoopings != null)
				{
					AnimationEvent[] events = childs[i].animClipLoopings.events;
					for (int j = 0; j < events.Length; j++)
					{
						events[j].stringParameter = info.ToString();
					}
					childs[i].animClipLoopings.events = events;
					clips.Add(childs[i].animClipLoopings);
					Debug.Log($"New Clip : {childs[i].animClipLoopings}");
				}
			}
			for (int i = 0; i < 5 - childs.Count; i++)
			{
				clips.Add(null);
			}

			to.anim.SetAnimationOverrides(
				new List<string>() { "Zero", "One", "Two", "Three", "Four", "Zero" + PlayerCast.DISOPERATE, "One" + PlayerCast.DISOPERATE, "Two" + PlayerCast.DISOPERATE, "Three" + PlayerCast.DISOPERATE, "Four" + PlayerCast.DISOPERATE ,
				"Zero" + PlayerCast.LOOP, "One" + PlayerCast.LOOP, "Two" + PlayerCast.LOOP, "Three" + PlayerCast.LOOP, "Four" + PlayerCast.LOOP},
				clips);

			(to.anim as PlayerAnim).curEquipped = this;
		}
	}

	public void NextCombo(bool circular = true, Actor self = null)
	{
		
		if(self != null)
			childs[curCombo].Disoperate(self);
		
		if (circular)
		{
			Debug.Log("다음콤보");
			curCombo += 1;
			curCombo %= childs.Count; 
			prevOperateSec = Time.time;
		}
		else if(curCombo < childs.Count - 1)
		{
			Debug.Log("다음콤보");
			curCombo += 1; 
			prevOperateSec = Time.time;
		}

		if (self.atk is PlayerAttack atk)
		{
			Debug.Log("각종강화효과지우기");
			atk.HandleRemoveCall();
		}
		if (isSuperArmor)
		{
			self.life.superArmor = false;
		}
	}

	public void ResetCombo()
	{
		curCombo = 0;
	}
}
