using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : AnimModule
{
	
	
	protected readonly int camStatHash = Animator.StringToHash("CamStat");
	protected readonly int atkStatHash = Animator.StringToHash("AtkStat");
	protected readonly int jumpHash = Animator.StringToHash("Jump");
	protected readonly int onAirHash = Animator.StringToHash("OnAir");
	protected readonly int interactHash = Animator.StringToHash("Interact");
	protected readonly int vertPowerHash = Animator.StringToHash("VertPower");
	protected readonly int aimSpeedHash = Animator.StringToHash("AimSpeed");
	protected readonly int interSpeedHash = Animator.StringToHash("InterSpeed");
	protected readonly int equipHash = Animator.StringToHash("Equip");
	protected readonly int unequipHash = Animator.StringToHash("Unequip");
	protected readonly int rollHash = Animator.StringToHash("Roll");

	protected readonly int skillAtomCountHash = Animator.StringToHash("SkillAtomCount");

	protected readonly int attack0Hash = Animator.StringToHash("Attack0");
	protected readonly int attack1Hash = Animator.StringToHash("Attack1");
	protected readonly int attack2Hash = Animator.StringToHash("Attack2");
	protected readonly int attack3Hash = Animator.StringToHash("Attack3");
	protected readonly int attack4Hash = Animator.StringToHash("Attack4");
	protected readonly int disop0Hash = Animator.StringToHash("Disop0");
	protected readonly int disop1Hash = Animator.StringToHash("Disop1");
	protected readonly int disop2Hash = Animator.StringToHash("Disop2");
	protected readonly int disop3Hash = Animator.StringToHash("Disop3");
	protected readonly int disop4Hash = Animator.StringToHash("Disop4");
	protected readonly int loopAfterHash = Animator.StringToHash("LoopAfter");


	PlayerMove pmove;

	internal Composite curEquipped;

	public override void Awake()
	{
		Animator[] anims = GetComponentsInChildren<Animator>();
		anim = anims[1];
	}

	private void Start()
	{
		pmove = GetActor().move as PlayerMove;
	}

	internal void SetLoopState()
	{
		anim.SetBool(loopAfterHash, true);
	}

	internal void ResetLoopState()
	{
		anim.SetBool(loopAfterHash, false);
	}

	private void LateUpdate()
	{
		if(!GetActor().move.idling)
		{
			switch (CameraManager.instance.curCamStat)
			{
				case CamStatus.Freelook:
					anim.SetFloat(moveXHash, 0);
					switch (GetActor().move.moveStat)
					{
						case MoveStates.Walk:
							anim.SetFloat(moveYHash, GetActor().move.walkSpeed);
							break;
						case MoveStates.Run:
							anim.SetFloat(moveYHash, GetActor().move.runSpeed);
							break;
						case MoveStates.Sit:
							anim.SetFloat(moveYHash, GetActor().move.crouchSpeed);
							break;
						case MoveStates.Climb:
							
							break;
						default:
							break;
					}
					break;
				case CamStatus.Locked:
					//anim.SetFloat(moveXHash, pmove.MoveDirCalced.x);
					//anim.SetFloat(moveYHash, pmove.MoveDirCalced.z);
					//break;
				case CamStatus.Aim:
					anim.SetFloat(moveXHash, pmove.MoveDirUncalced.x);
					anim.SetFloat(moveYHash, pmove.MoveDirUncalced.z);
					break;
				default:
					break;
			}
		}

		anim.SetBool(onAirHash, pmove.onAir);
		anim.SetFloat(vertPowerHash, pmove.MoveDirUncalced.y);
	}

	public void SetAttackTrigger(int idx)
	{
		switch (idx)
		{
			case 0:
				anim.SetTrigger(attack0Hash);
				break;
			case 1:
				anim.SetTrigger(attack1Hash);
				break;
			case 2:
				anim.SetTrigger(attack2Hash);
				break;
			case 3:
				anim.SetTrigger(attack3Hash);
				break;
			case 4:
				anim.SetTrigger(attack4Hash);
				break;
		}
	}

	public void SetDisopTrigger(int idx)
	{
		switch (idx)
		{
			case 0:
				anim.SetTrigger(disop0Hash);
				break;
			case 1:
				anim.SetTrigger(disop1Hash);
				break;
			case 2:
				anim.SetTrigger(disop2Hash);
				break;
			case 3:
				anim.SetTrigger(disop3Hash);
				break;
			case 4:
				anim.SetTrigger(disop4Hash);
				break;
		}
	}

	public void SetJumpTrigger()
	{
		anim.SetTrigger(jumpHash);
	}

	public void SetInteractTrigger()
	{
		anim.SetTrigger(interactHash);
	}

	public void SetAttackState(int val)
	{
		anim.SetInteger(atkStatHash, val);
	}

	public void SetSkillAtomCount(int val)
	{
		anim.SetInteger(skillAtomCountHash, val);
	}

	public void SetAimSpeed(float val)
	{
		anim.SetFloat(aimSpeedHash, val);
	}

	public void SetInterSpeed(float val)
	{
		anim.SetFloat(interSpeedHash, val);
	}

	public void SetEquipTrigger()
	{
		anim.SetTrigger(equipHash);
	}

	public void SetUnequipTrigger()
	{
		anim.SetTrigger(unequipHash);
	}

	public void SetRollTrigger()
	{
		anim.SetTrigger(rollHash);
	}
}
