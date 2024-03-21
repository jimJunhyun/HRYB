using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 항상 1번 슬롯에 장착된 스킬의 opAnim, durAnim, DisopAnim을 사용할 것.
/// 여러개를 꽂기보다는 여러개를 컴포지트로 묶고 꽂기.
/// 
/// 일정 시간 충전한ㄴ 뒤 묶여있는 스킬들을 전부 사용하는 형태.
/// 충전중엔 모으기 말고 아무것도 안함.
/// 
/// 일정 시간 모으기가 발동하지 않으면 사용할 수 없는 스킬에 어울림.
/// </summary>

[CreateAssetMenu(menuName = "Skills/ChargeCastRoot")]
public class ChargeCastRoot : SkillRoot
{
	public float maxChargeSec;
	public float chargeThreshold;


	bool charging = false;
	float chargeStartSec;

	Actor owner;

	float chargeT => Time.time - chargeStartSec;
	bool overcooked => chargeT >= maxChargeSec;
	bool prepared => chargeT >= chargeThreshold && !overcooked;


	public override void Operate(Actor self)
	{
		if (isPlayAnim)
		{
			if (self.anim is PlayerAnim pa)
			{
				pa.SetAttackTrigger(0); //애니메이션트리거로 PauseAnimation 및 MyOperation (SetAttackRange) 발동
				charging = true;
				chargeStartSec = Time.time;
				GameManager.instance.uiManager.interingUI.On();
			}
		}
		else
		{
			MyOperation(self);
		}
		owner = self;
	}

	public override void Disoperate(Actor self)
	{
		if (isPlayDisopAnim)
		{
			if(self.anim is PlayerAnim pa)
			{
				pa.ResetLoopState();
				pa.SetDisopTrigger(0);
				charging = false;
				GameManager.instance.uiManager.interingUI.Off();
			}
		}
		else
		{
			MyDisoperation(self);
		}
		owner = null;
	}

	internal override void MyOperation(Actor self)
	{
		for (int i = 0; i < childs.Count; i++)
		{
			childs[i].Operate(self);
		}
	}

	internal override void MyDisoperation(Actor self)
	{
		if (prepared)
		{
			GameManager.instance.StartCoroutine(DelDisoperate(self));
		}
	}

	public override void UpdateStatus()
	{
		base.UpdateStatus();
		if (charging)
		{
			GameManager.instance.uiManager.interingUI.SetGaugeValue(chargeT / chargeThreshold);
		}
		if (overcooked)
		{
			Disoperate(owner);
		}
	}

	protected override IEnumerator DelDisoperate(Actor self)
	{
		yield return base.DelDisoperate(self);
		if (self.atk is PlayerAttack atk)
		{
			Debug.Log("각종강화효과지우기");
			atk.HandleRemoveCall();
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
			to.anim.SetAnimationOverrides(new List<string>() { 
				"Zero", "One", "Two", "Three", "Four",
				"Zero" + PlayerCast.DISOPERATE, "One" + PlayerCast.DISOPERATE, "Two" + PlayerCast.DISOPERATE, "Three" + PlayerCast.DISOPERATE, "Four" + PlayerCast.DISOPERATE,
				"Zero" + PlayerCast.LOOP, "One" + PlayerCast.LOOP, "Two" + PlayerCast.LOOP, "Three" + PlayerCast.LOOP, "Four" + PlayerCast.LOOP}, clips);

			(to.anim as PlayerAnim).curEquipped = this;
		}
	}
}
