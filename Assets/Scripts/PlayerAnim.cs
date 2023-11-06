using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : AnimModule
{
	
	protected readonly int camStatHash = Animator.StringToHash("CamStat");
	protected readonly int atkStatHash = Animator.StringToHash("AtkStat");

	public override void Awake()
	{
		anim = GetComponentInChildren<Animator>();
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
						default:
							break;
					}
					break;
				case CamStatus.Locked:
					anim.SetFloat(moveXHash, GetActor().move.MoveVelocity.x);
					anim.SetFloat(moveYHash, GetActor().move.MoveVelocity.z);
					break;
				case CamStatus.Aim:
					anim.SetFloat(moveXHash, GetActor().move.MoveVelocity.x);
					anim.SetFloat(moveYHash, GetActor().move.MoveVelocity.z);
					break;
				default:
					break;
			}
			anim.SetInteger(moveHash, ((int)GetActor().move.moveStat));
			anim.SetBool(idleHash, false);
		}
		else
		{
			anim.SetBool(idleHash, true);
			anim.SetInteger(moveHash, ((int)GetActor().move.moveStat));
		}

		anim.SetInteger(atkStatHash, ((int)GetActor().atk.attackState));
	}
}
