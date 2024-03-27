using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 항상 1번 슬롯에 장착된 스킬의 opAnim, durAnim, DisopAnim을 사용함.
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
	bool overcooked => charging && chargeT >= maxChargeSec;
	bool prepared => chargeT >= chargeThreshold && !overcooked;


	public override void Operate(Actor self)
	{
		if (isSuperArmor)
		{
			self.life.superArmor = true;
		}

		if (isPlayAnim)
		{
			if (self.anim is PlayerAnim pa)
			{
				pa.SetAttackTrigger(0); //애니메이션트리거로 PauseAnimation 및 MyOperation (SetAttackRange) 발동
				//chargeStartSec = Time.time;
				
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
		charging  = true;
		GameManager.instance.uiManager.interingUI.On();
		chargeStartSec = Time.time;
	}

	internal override void MyDisoperation(Actor self)
	{
		if (prepared)
		{
			GameManager.instance.StartCoroutine(DelDisoperate(self));
		}
		else
		{
			for (int i = 0; i < childs.Count; i++)
			{
				ActualDisoperateAt(self, i);
			}
		}
		GameManager.instance.uiManager.interingUI.Off();
		Debug.Log("충전종료, 스킬을 사용했는가? : " + prepared);
		charging = false;
	}

	public override void UpdateStatus()
	{
		base.UpdateStatus();
		if (charging)
		{
			GameManager.instance.uiManager.interingUI.SetGaugeValue(chargeT / chargeThreshold);
			//Debug.Log($"충전중우 : {chargeT} / {chargeThreshold} = " + chargeT / chargeThreshold);
		}
		if (overcooked)
		{
			Disoperate(owner);
		}
	}

	protected override IEnumerator DelDisoperate(Actor self)
	{
		if (isPlayDisopAnim)
		{
			//(GameManager.instance.pActor.anim as PlayerAnim).SetDisopTrigger(0);
			for (int i = 0; i < childs.Count; i++)
			{
				childs[i].Disoperate(self);
				yield return new WaitForSeconds(composeDel);
			}
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

	public override void SetAnimations(Actor to, SkillSlotInfo info)
	{
		if ((to.anim as PlayerAnim).curEquipped != this)
		{
			List<AnimationClip> clips = new List<AnimationClip>();
			if (childs[0].animClip != null)
			{
				AnimationEvent[] events = childs[0].animClip.events;
				for (int j = 0; j < events.Length; j++)
				{
					events[j].stringParameter = info.ToString();

				}
				childs[0].animClip.events = events;
				clips.Add(childs[0].animClip);
				Debug.Log($"New Clip : {childs[0].animClip} for op");
			}
			
			for (int i = 0; i < 4; i++)
			{
				clips.Add(null);
			}

			if (childs[0].animClipDisop != null)
			{
				AnimationEvent[] events = childs[0].animClipDisop.events;
				for (int j = 0; j < events.Length; j++)
				{
					events[j].stringParameter = info.ToString();

				}
				childs[0].animClipDisop.events = events;
				clips.Add(childs[0].animClipDisop);
				Debug.Log($"New Clip : {childs[0].animClipDisop} for Disop");
			}
			
			for (int i = 0; i < 4; i++)
			{
				clips.Add(null);
			}
			//dur
			if (childs[0].animClipLoopings != null)
			{
				AnimationEvent[] events = childs[0].animClipLoopings.events;
				for (int j = 0; j < events.Length; j++)
				{
					events[j].stringParameter = info.ToString();
				}
				childs[0].animClipLoopings.events = events;
				clips.Add(childs[0].animClipLoopings);
				Debug.Log($"New Clip : {childs[0].animClipLoopings} for LOOP");
			}
			
			for (int i = 0; i < 4; i++)
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
