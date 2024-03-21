using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using Quaternion = System.Numerics.Quaternion;

public class PlayerAnimActions : MonoBehaviour
{
	Actor self;

	[SerializeField] private AnimatorOverrideController _anim;
	//GameObject holdingBow;
	//GameObject equipingBow;
	public SkinnedMeshRenderer eBowRend;

	Animator animator;

	readonly int aimHash = Animator.StringToHash("Aim");
	readonly int fireHash = Animator.StringToHash("Fire");

	private void Awake()
	{
		self = GetComponentInParent<Actor>();
		animator = GetComponent<Animator>();
		animator.runtimeAnimatorController = _anim;

		//holdingBow  = GameObject.Find("HoldingBow");
		//equipingBow = GameObject.Find("EquipingBow");

		//eBowRend = equipingBow.GetComponentInChildren<SkinnedMeshRenderer>();

		//animator = holdingBow.GetComponent<Animator>();
		//BowUnequip();
	}


	private void Update()
	{
		transform.localPosition = new Vector3(0, 0, 0);
		transform.localRotation = UnityEngine.Quaternion.identity;
	}

	public void LoadArrow()
	{
		(self.atk as PlayerAttack).SetBowStat();
	}

	public void FireArrow()
	{
		(self.cast as PlayerCast).ActualSkillOperate(SkillSlotInfo.LClick);
	}

	public void DoAttack(AnimationEvent evt)
	{
		string[] str = evt.stringParameter.Split('$');
		SkillSlotInfo info;
		if(str.Length > 1)
		{
			info = System.Enum.Parse<SkillSlotInfo>(str[1]);
		}
		else
		{
			info = System.Enum.Parse<SkillSlotInfo>(evt.stringParameter.Trim('$'));
		}
		(self.cast as PlayerCast).ActualSkillOperate(info);
	}

	public void SetAttackRange(AnimationEvent evt)
	{
		Debug.Log(evt.animatorClipInfo.clip.name + " : " + evt.stringParameter);
		string[] str = evt.stringParameter.Split('$');
		SkillSlotInfo info;
		if (str.Length > 1)
		{
			info = System.Enum.Parse<SkillSlotInfo>(str[1]);
		}
		else
		{
			info = System.Enum.Parse<SkillSlotInfo>(evt.stringParameter.Trim('$'));
		}
		(self.cast as PlayerCast).ActualSkillOperate(info, evt.intParameter);
	}

	public void ResetAttackRange(AnimationEvent evt)
	{
		string[] str = evt.stringParameter.Split('$');
		SkillSlotInfo info;
		if (str.Length > 1)
		{
			info = System.Enum.Parse<SkillSlotInfo>(str[1]);
		}
		else
		{
			info = System.Enum.Parse<SkillSlotInfo>(evt.stringParameter.Trim('$'));
		}
		(self.cast as PlayerCast).ActualSkillDisoperate(info, evt.intParameter);
	}

	public void StopAttack(AnimationEvent evt)
	{
		SkillSlotInfo info = System.Enum.Parse<SkillSlotInfo>(evt.stringParameter);
		(self.cast as PlayerCast).ActualSkillDisoperate(info);
	}

	public void PauseAnimation()
	{
		(self.anim as PlayerAnim).SetLoopState();
	}

	public void ResumeAnimation()
	{
		(self.anim as PlayerAnim).ResetLoopState();
	}

	public void DisableInput()
	{
		//GameManager.instance.pinp.DeactivateInput();
		
		//self.move.forceDir = Vector3.zero;
		DisableMove();
	}

	public void DisableMove()
	{
		GameManager.instance.DisableCtrl(ControlModuleMode.Animated);
	}

	public void EnableMove()
	{
		GameManager.instance.EnableCtrl(ControlModuleMode.Animated);
	}

	public void EnableInput()
	{
		GameManager.instance.pinp.ActivateInput();
		EnableMove();
	}

	//public void BowEquip()
	//{
	//	holdingBow.SetActive(true);
	//	equipingBow.SetActive(false);
	//}
	//
	//public void BowUnequip()
	//{
	//	holdingBow.SetActive(false);
	//	equipingBow.SetActive(true);
	//}

	//public void SetBowAimState()
	//{
	//	animator.SetBool(aimHash, true);
	//}
	//
	//public void ResetBowAimState()
	//{
	//	animator.SetBool(aimHash, false);
	//}
	//
	//public void SetFireTrigger()
	//{
	//	animator.SetTrigger(fireHash);
	//}

	public void ResetRoll()
	{
		EnableInput();
		self.life.isImmune = false;
		self.move.moveDir = Vector3.zero;
		(self.move as PlayerMove).ctrl.height *= 2f;
		(self.move as PlayerMove).ctrl.radius *= 2f;
		if (self.move.moveStat != MoveStates.Sit)
			(self.move as PlayerMove).ctrl.center *= 2f;
	}

	public void OnAnimationStart(AnimationEvent evt)
	{
		if ((self.cast as PlayerCast).NowSkillUse != null)
			(self.cast as PlayerCast).NowSkillUse.OnAnimationStart(self, evt);

	}

	public void OnAnimationMove(AnimationEvent evt)
	{
		if ((self.cast as PlayerCast).NowSkillUse != null)
		{
			(self.cast as PlayerCast).NowSkillUse.OnAnimationMove(self, evt);
		}
	}

	public void OnAnimationEvent(AnimationEvent evt)
	{
		if ((self.cast as PlayerCast).NowSkillUse != null)
			(self.cast as PlayerCast).NowSkillUse.OnAnimationEvent(self, evt);
	}

	public void OnAnimationStop(AnimationEvent evt)
	{
		if ((self.cast as PlayerCast).NowSkillUse != null)
			(self.cast as PlayerCast).NowSkillUse.OnAnimationStop(self, evt);
	}

	public void OnAnimationEnd(AnimationEvent evt)
	{
		if ((self.cast as PlayerCast).NowSkillUse != null)
			(self.cast as PlayerCast).NowSkillUse.OnAnimationEnd(self, evt);
	}
}
