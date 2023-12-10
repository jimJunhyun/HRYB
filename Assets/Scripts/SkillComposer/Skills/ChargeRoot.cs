using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/ChargeRoot")]
public class ChargeRoot : SkillRoot
{
	public float secPerCharge;

	public bool isAimMode = false;

	int curCharge = 0;
	float chargeStartSec;

	bool charging = false;

	Actor owner;

	public override void Disoperate(Actor self)
	{
		if (charging)
		{
			Debug.Log("Charge Ended");
			charging = false;
			base.Disoperate(self);
			owner = null;
			if(self.anim is PlayerAnim pa)
			{
				pa.SetDisopTrigger(curCharge);
			}
			if (isAimMode)
			{
				GameManager.instance.SwitchTo(CamStatus.Freelook);
				GameManager.instance.uiManager.aimUI.Off();
			}
		}
	}

	public override void Operate(Actor self)
	{
		if (!charging)
		{
			charging = true;
			chargeStartSec = Time.time;
			curCharge = 0;
			Debug.Log($"Charge Started, 1/{childs.Count}");
			childs[curCharge].Operate(owner);
			owner = self;
			if (isAimMode)
			{
				GameManager.instance.SwitchTo(CamStatus.Aim);
				GameManager.instance.uiManager.aimUI.On();
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

	public override void SetAnimations(Actor to)
	{
		if ((to.anim as PlayerAnim).curEquipped != this)
		{
			List<AnimationClip> clips = new List<AnimationClip>();
			for (int i = 0; i < childs.Count; i++)
			{
				if (childs[i].animClip != null)
				{
					clips.Add(childs[i].animClip);
					Debug.Log($"New Clip : {childs[i].animClip}");
				}
			}
			to.anim.SetAnimationOverrides(new List<string>() { "Zero", "One", "Two", "Three", "Four" }, clips);

			clips.Clear();
			for (int i = 0; i < childs.Count; i++)
			{
				if (childs[i].animClipDisop != null)
				{
					clips.Add(childs[i].animClipDisop);
					Debug.Log($"New Clip : {childs[i].animClipDisop}");
				}

			}
			to.anim.SetAnimationOverrides(new List<string>() { "Zero" + PlayerCast.DISOPERATE, "One" + PlayerCast.DISOPERATE, "Two" + PlayerCast.DISOPERATE, "Three" + PlayerCast.DISOPERATE, "Four" + PlayerCast.DISOPERATE }, clips);

			(to.anim as PlayerAnim).curEquipped = this;
		}
	}

	internal override void MyDisoperation(Actor self)
	{
		//Do nothing
	}

	internal override void MyOperation(Actor self)
	{
		//Do nothing
	}
}
