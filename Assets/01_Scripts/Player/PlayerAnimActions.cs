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

	SkinnedMeshRenderer hair;
	SkinnedMeshRenderer ear;
	SkinnedMeshRenderer tail;
	SkinnedMeshRenderer head;

	GameObject foxCloth;
	GameObject humanCloth;

	public Material[] hairMats;
	public Material[] eyeMats;

	PlayerForm form;

	Animator animator;

	readonly int aimHash = Animator.StringToHash("Aim");
	readonly int fireHash = Animator.StringToHash("Fire");

	private void Awake()
	{
		self = GetComponentInParent<Actor>();
		animator = GetComponent<Animator>();
		animator.runtimeAnimatorController = _anim;

		hair = transform.Find("Rad_Hair").GetComponent<SkinnedMeshRenderer>();
		head = transform.Find("Body").GetComponent<SkinnedMeshRenderer>();
		ear = transform.Find("Rad_Kemomimi").GetComponent<SkinnedMeshRenderer>();
		tail = transform.Find("Rad_Tail").GetComponent<SkinnedMeshRenderer>();
		foxCloth = transform.Find("FoxCloth").gameObject;
		humanCloth = transform.Find("HumanCloth").gameObject;


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
		string[] strs = evt.stringParameter.Split('$');
		SkillSlotInfo info;
		if (strs.Length > 1)
		{
			info = System.Enum.Parse<SkillSlotInfo>(strs[1]);
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
		string[] strs = evt.stringParameter.Split('$');
		SkillSlotInfo info;
		if (strs.Length > 1)
		{
			info = System.Enum.Parse<SkillSlotInfo>(strs[1]);
		}
		else
		{
			info = System.Enum.Parse<SkillSlotInfo>(evt.stringParameter.Trim('$'));
		}
		(self.cast as PlayerCast).ActualSkillOperate(info, evt.intParameter);
	}

	public void ResetAttackRange(AnimationEvent evt)
	{
		string[] strs = evt.stringParameter.Split('$');
		SkillSlotInfo info;
		if (strs.Length > 1)
		{
			info = System.Enum.Parse<SkillSlotInfo>(strs[1]);
		}
		else
		{
			info = System.Enum.Parse<SkillSlotInfo>(evt.stringParameter.Trim('$'));
		}
		(self.cast as PlayerCast).ActualSkillDisoperate(info, evt.intParameter);
	}

	public void StopAttack(AnimationEvent evt)
	{
		string[] strs = evt.stringParameter.Split('$');
		SkillSlotInfo info;
		if (strs.Length > 1)
		{
			info = System.Enum.Parse<SkillSlotInfo>(strs[1]);
		}
		else
		{
			info = System.Enum.Parse<SkillSlotInfo>(evt.stringParameter.Trim('$'));
		}
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
		GameManager.instance.DisableCtrl();
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

	public void ChangeForm()
	{
		switch (form)
		{
			case PlayerForm.Magic:
				ChangeFormTo(PlayerForm.Yoho);
				break;
			case PlayerForm.Yoho:
				ChangeFormTo(PlayerForm.Magic);
				break;
			default:
				break;
		}
	}

	public void ChangeFormTo(PlayerForm f)
	{
		form = f;
		switch (form)
		{
			case PlayerForm.Magic:
				tail.enabled = false;
				ear.enabled = false;
				hair.material = hairMats[((int)PlayerForm.Magic)];
				head.materials[1] = eyeMats[((int)PlayerForm.Magic)];
				foxCloth.SetActive(false);
				humanCloth.SetActive(true);
				break;
			case PlayerForm.Yoho:
				tail.enabled = true;
				ear.enabled = true;
				hair.material = hairMats[((int)PlayerForm.Yoho)];
				head.materials[1] = eyeMats[((int)PlayerForm.Yoho)];
				foxCloth.SetActive(true);
				humanCloth.SetActive(false);
				break;
			default:
				break;
		}
		
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

		if(evt.stringParameter == "Foot")
		{

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
