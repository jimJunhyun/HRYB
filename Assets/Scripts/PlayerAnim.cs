using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : AnimModule
{
	
	protected readonly int camStatHash = Animator.StringToHash("CamStat");

	public float idleDetailT = 2;

	float curDetailWaiter = 0;

	public override void Awake()
	{
		curDetailWaiter = 0;
		anim = GetComponentInChildren<Animator>();
	}

	private void LateUpdate()
	{
		if(GetActor().move.moveDir.sqrMagnitude > 0.1f)
		{
			switch (GameManager.instance.curCamStat)
			{
				case CamStatus.Freelook:
					anim.SetFloat(moveXHash, 0);
					anim.SetFloat(moveYHash, 1);
					break;
				case CamStatus.Locked:
					anim.SetFloat(moveXHash, self.move.moveDir.x);
					anim.SetFloat(moveYHash, self.move.moveDir.z);
					break;
				case CamStatus.Aim:
					anim.SetFloat(moveXHash, self.move.moveDir.x);
					anim.SetFloat(moveYHash, self.move.moveDir.z);
					break;
				default:
					break;
			}
			anim.SetInteger(moveHash, ((int)GetActor().move.moveStat));
		}
		
	}
}
