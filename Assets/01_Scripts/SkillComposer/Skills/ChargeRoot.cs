using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 충전하면서 N초마다 지속적으로 무언가를 해주다가, 
/// 충전을 종료할 경우 자신 모든 자식을 종료해주는 역할.
/// 
/// 누르고있을때마다 모이고, 떼서 발사, 위력이나 발사 수 등이 달라지는 계열에 적합함.
/// </summary>
[CreateAssetMenu(menuName = "Skills/ChargeRoot")]
public class ChargeRoot : SkillRoot
{
	public float secPerCharge;

	public bool isAimMode = false;

	int curCharge = 0;
	float chargeStartSec;

	bool charging = false;

	Actor owner;

	float prevOperateSec;

	public override void Disoperate(Actor self)
	{
		if (isPlayDisopAnim)
		{
			if (self.anim is PlayerAnim pa)
			{
				pa.SetDisopTrigger(curCharge);
				pa.ResetLoopState();
			}
			if (isAimMode)
			{
				//CameraManager.instance.SwitchTo(CamStatus.Freelook);
				GameManager.instance.uiManager.aimUI.Off();
			}
		}
		else
		{
			MyDisoperation(self);
		}
		
	}

	public override void Operate(Actor self)
	{
		if (Time.time - prevOperateSec >= composeDel)
		{
			if (isSuperArmor)
			{
				self.life.superArmor = true;
			}
			if (self.anim is PlayerAnim pa)
			{
				GameManager.instance.uiManager.aimUI.On();
				pa.SetAttackTrigger(0);
			}
		}
		
	}

	public override void UpdateStatus()
	{
		if (charging && Time.time - chargeStartSec >= secPerCharge && curCharge < childs.Count - 1)
		{
			curCharge += 1;
			Debug.Log($"충전 {curCharge + 1}/{childs.Count}");
			childs[curCharge].Operate(owner);
			chargeStartSec = Time.time;
		}
		base.UpdateStatus();
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
					if(events.Length > 1)
					{
						events[1].stringParameter = info.ToString();
					}
					if(events.Length > 2)
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

	internal override void MyDisoperation(Actor self)
	{
		if (charging)
		{
			(self.anim as PlayerAnim).ResetLoopState();
			Debug.Log("Charge Ended");
			charging = false;
			//base.Disoperate(self);
			GameManager.instance.StartCoroutine(DelDisoperater(self));

			owner = null;
			if (isAimMode)
			{
				GameManager.instance.uiManager.aimUI.Off();
			}
			curCharge = 0;
		}
	}

	internal override void MyOperation(Actor self)
	{
		if (!charging)
		{
			charging = true;
			chargeStartSec = Time.time;
			curCharge = 0;
			Debug.Log($"CHARGE STARTED, 1/{childs.Count}");
			childs[curCharge].Operate(owner);
			owner = self;
			if (isAimMode)
			{
				GameManager.instance.uiManager.aimUI.On();
			}
		}
	}

	IEnumerator DelDisoperater(Actor self)
	{
		for (int i = 0; i < childs.Count; i++)
		{
			childs[i].Disoperate(self);
			yield return new WaitForSeconds(composeDel);
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
}
