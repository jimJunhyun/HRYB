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

	PlayerMove pmove;

	public override void Awake()
	{
		anim = GetComponentInChildren<Animator>();
		
	}

	private void Start()
	{
		pmove = GetActor().move as PlayerMove;
	}

	private void LateUpdate()
	{
		if(!GetActor().move.idling)
		{
			switch (GameManager.instance.curCamStat)
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
