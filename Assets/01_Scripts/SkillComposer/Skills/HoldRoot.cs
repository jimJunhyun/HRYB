using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/HoldRoot")]
public class HoldRoot : SkillRoot
{
	//public float holdingMaxSec;

	public List<int> nextChildOps;

	public float clickBanSec = 5;

	public int maxAmmo = 35;

	public float reloadSec = 0.2f;

	bool holding = false;

	//float holdingStartSec;
	//float prevUpgradeSec;

	float prevOperateSec;

	float prevReloadSec;

	int curMode = 0;

	int ammo = 35;

	int operCnt = 0;

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
		}
		MyDisoperation(self);

	}

	public override void Operate(Actor self)
	{
		if (Time.time - prevOperateSec >= composeDel)
		{
			if (self.anim is PlayerAnim pa)
			{
				MyOperation(self);
				//prevUpgradeSec = Time.time;
			}
		}

	}

	public override void UpdateStatus()
	{
		if (holding && Time.time - prevOperateSec >= composeDel)
		{
			childs[curMode].Operate(owner);
			++operCnt;
			ammo -= 1;
			prevOperateSec = Time.time;
			if(curMode + 1 < childs.Count && nextChildOps[curMode] > 0 && operCnt >= nextChildOps[curMode] /*Time.time - prevUpgradeSec >= nextChildSecs[curMode]*/)
			{
				operCnt -= nextChildOps[curMode];
				curMode += 1;
				Debug.Log($"과열 {curMode + 1}/{childs.Count}, 단계 업그레이드");
				//prevUpgradeSec = Time.time;
			}
		}
		base.UpdateStatus();
		if(ammo < 1)
		{
			Disoperate(owner);
		}

		if(!holding && Time.time - prevReloadSec >= reloadSec && ammo < maxAmmo)
		{
			ammo += 1;
			Debug.Log($"장전 {ammo}/{maxAmmo}");
			prevReloadSec = Time.time;
		}
	}


	internal override void MyDisoperation(Actor self)
	{
		if (holding)
		{
			Debug.Log("hold Ended");
			holding = false;

			if(curMode == childs.Count - 1 && self.atk is PlayerAttack pa)
			{
				Debug.Log("PAUSED FOR : " + clickBanSec);
				pa.NoClick.Pause(ControlModuleMode.Status, true, clickBanSec);
			}

			owner = null;
			curMode = 0;
			operCnt = 0;
			if (self.move is PlayerMove pm)
			{
				pm.moveModuleStat.HandleSpeed(-0.5f, ModuleController.SpeedMode.Slow);
			}
		}
	}

	internal override void MyOperation(Actor self)
	{
		if (!holding)
		{
			holding = true;

			curMode = 0;
			Debug.Log($"과열, 1/{childs.Count}, 발사시작");
			//childs[curMode].Operate(self);
			owner = self;

			if(self.move is PlayerMove pm)
			{
				pm.moveModuleStat.HandleSpeed(0.5f, ModuleController.SpeedMode.Slow);
			}
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
