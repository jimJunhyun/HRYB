using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/HoldRoot")]
public class HoldRoot : SkillRoot
{
	public float holdingMaxSec;

	public List<float> nextChildSecs;

	bool holding = false;

	float holdingStartSec;
	float prevUpgradeSec;

	float prevOperateSec;

	int curMode = 0;

	Actor owner;

	public override void Disoperate(Actor self)
	{
		if (isPlayDisopAnim)
		{
			if (self.anim is PlayerAnim pa)
			{
				pa.SetDisopTrigger(curMode);
				pa.ResetStopState();
			}
			MyDisoperation(self);
		}

	}

	public override void Operate(Actor self)
	{
		if (Time.time - prevOperateSec >= composeDel)
		{
			if (self.anim is PlayerAnim pa)
			{
				pa.SetAttackTrigger(0);
			}
		}

	}

	public override void UpdateStatus()
	{
		if (holding && Time.time - prevOperateSec >= composeDel)
		{
			childs[curMode].Operate(owner);
			prevOperateSec = Time.time;
			if(curMode + 1 < childs.Count && nextChildSecs[curMode] > 0 && Time.time - prevUpgradeSec >= nextChildSecs[curMode])
			{
				Debug.Log($"과열 {curMode + 1}/{childs.Count}");
				curMode += 1;
				prevUpgradeSec = Time.time;
			}
		}
		base.UpdateStatus();
		if(Time.time - holdingStartSec >= holdingMaxSec)
		{
			Disoperate(owner);
		}
	}


	internal override void MyDisoperation(Actor self)
	{
		if (holding)
		{
			Debug.Log("hold Ended");
			holding = false;

			owner = null;
			curMode = 0;
		}
	}

	internal override void MyOperation(Actor self)
	{
		if (!holding)
		{
			holding = true;
			holdingStartSec = Time.time;
			curMode = 0;
			Debug.Log($"과열, 1/{childs.Count}");
			childs[curMode].Operate(owner);
			owner = self;
		}
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
					AnimationEvent[] events = childs[i].animClip.events;
					for (int j = 0; j < events.Length; j++)
					{
						events[j].stringParameter = info.ToString();
					}

					childs[i].animClip.events = events;
					clips.Add(childs[i].animClip);
					Debug.Log($"New Clip : {childs[i].animClip}");
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
					AnimationEvent[] events = childs[i].animClipDisop.events;
					for (int j = 0; j < events.Length; j++)
					{
						events[j].stringParameter = info.ToString();
					}
					childs[i].animClipDisop.events = events;
					clips.Add(childs[i].animClipDisop);
					Debug.Log($"New Clip : {childs[i].animClipDisop}");
				}
			}
			for (int i = 0; i < 5 - childs.Count; i++)
			{
				clips.Add(null);
			}
			to.anim.SetAnimationOverrides(new List<string>() { "Zero", "One", "Two", "Three", "Four", "Zero" + PlayerCast.DISOPERATE, "One" + PlayerCast.DISOPERATE, "Two" + PlayerCast.DISOPERATE, "Three" + PlayerCast.DISOPERATE, "Four" + PlayerCast.DISOPERATE }, clips);

			(to.anim as PlayerAnim).curEquipped = this;
		}
	}
}
