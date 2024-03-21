using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AreaShapeMode
{
	None,
	Circle,
	Rectangle,
	Triangle,

}

/// <summary>
/// 항상 1번 슬롯에 장착된 스킬의 opAnim, durAnim, DisopAnim을 사용할 것.
/// 여러개를 꽂기보다는 여러개를 컴포지트로 묶고 꽂기.
/// </summary>

public class ChargeCastRoot : SkillRoot
{

	public float maxChargeSec;
	public float chargeThreshold;
	
	public float maxDist;

	public AreaShapeMode shape;

	bool charging = false;
	float chargeStartSec;

	float chargeT => Time.time - chargeStartSec;
	bool overcooked => chargeT >= maxChargeSec;
	bool prepared => chargeT >= chargeThreshold && !overcooked;


	public override void Operate(Actor self)
	{
		if (self.anim is PlayerAnim pa)
		{
			pa.SetAttackTrigger(0);
			pa.SetLoopState();
		}
	}

	public override void Disoperate(Actor self)
	{
		if (isPlayDisopAnim)
		{
			if(self.anim is PlayerAnim pa)
			{
				pa.ResetLoopState();
				pa.SetDisopTrigger(0);
			}
		}
		else
		{
			MyDisoperation(self);
		}
	}

	internal override void MyOperation(Actor self)
	{
		base.MyOperation(self);
	}

	internal override void MyDisoperation(Actor self)
	{
		base.MyDisoperation(self);
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
					if (events.Length > 1)
					{
						events[1].stringParameter = info.ToString();
					}
					if (events.Length > 2)
					{
						events[2].stringParameter = info.ToString();
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
					if (events.Length > 1)
					{
						events[1].stringParameter = info.ToString();
					}
					if (events.Length > 2)
					{
						events[2].stringParameter = info.ToString();
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
			//dur
			for (int i = 0; i < childs.Count; i++)
			{
				if (childs[i].animClipLoop != null)
				{
					AnimationEvent[] events = childs[i].animClipLoop.events;
					for (int j = 0; j < events.Length; j++)
					{
						events[j].stringParameter = info.ToString();
					}
					childs[i].animClipLoop.events = events;
					clips.Add(childs[i].animClipLoop);
					Debug.Log($"New Clip : {childs[i].animClipLoop}");
				}
			}
			for (int i = 0; i < 5 - childs.Count; i++)
			{
				clips.Add(null);
			}
			to.anim.SetAnimationOverrides(new List<string>() { 
				"Zero", "One", "Two", "Three", "Four",
				"Zero" + PlayerCast.DISOPERATE, "One" + PlayerCast.DISOPERATE, "Two" + PlayerCast.DISOPERATE, "Three" + PlayerCast.DISOPERATE, "Four" + PlayerCast.DISOPERATE,
				"Zero" + PlayerCast.LOOP, "One" + PlayerCast.LOOP, "Two" + PlayerCast.LOOP, "Three" + PlayerCast.LOOP, "Four" + PlayerCast.LOOP}, clips);

			(to.anim as PlayerAnim).curEquipped = this;
		}
	}
}
