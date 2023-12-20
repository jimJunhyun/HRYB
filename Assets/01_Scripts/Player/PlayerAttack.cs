using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.VFX;


public class PlayerAttack : AttackModule
{
	protected float secPerCharge;
	public float initSecPerCharge;

	public float SecPerCharge { get => secPerCharge; }

	//VisualEffect chargeEff;
	Ray camRay;
	public Transform ShootPos { get; protected set;}

	UnityAction updateActs;

	float AimTime { get => Time.time - aimStart;}

	float aimStart = 0;

	Coroutine ongoingResetter;

	bool clickL = false;
	bool clickR = false;

	PlayerAnimActions animActions;

	private void Awake()
	{
		ShootPos = GameObject.Find("ShootPos").transform;

		animActions = GetComponentInChildren<PlayerAnimActions>();

		//chargeEff = shootPos.GetComponentInChildren<VisualEffect>();
		//chargeEff.Stop();
	}

	private void Start()
	{
		BowDown();
	}

	private void Update()
	{
		updateActs?.Invoke();
		if (Mouse.current.rightButton.wasPressedThisFrame)
		{
			clickR = true;
			Debug.Log("AIM START");
			(GetActor().cast as PlayerCast).SetSkillUse(SkillSlotInfo.RClick);
		}
		if (Mouse.current.rightButton.wasReleasedThisFrame)
		{
			clickR = false;
			(GetActor().cast as PlayerCast).ResetSkillUse(SkillSlotInfo.RClick);
		}
	}

	public void OnAim(InputAction.CallbackContext context)
	{
		if (GameManager.instance.pinven.stat == HandStat.Weapon)
		{
			if (!clickL)
			{
				if (!clickR)
				{
					if (context.started)
					{
						
					}
					
				}
				else
				{
					if (context.canceled)
					{
						
					}
					
				}
			}
			
		}
	}

	public void OnAttack(InputAction.CallbackContext context)
	{
		//(GetActor().cast as PlayerCast).ResetSkillUse(SkillSlotInfo.RClick);
		
		if (GameManager.instance.pinven.stat == HandStat.Weapon)
		{
			if (!clickR)
			{
				if (context.started && !clickL)
				{
					clickL = true;
					(GetActor().cast as PlayerCast).SetSkillUse(SkillSlotInfo.LClick);
				}
				else if (context.canceled && clickL)
				{
					clickL = false;
					(GetActor().cast as PlayerCast).ResetSkillUse(SkillSlotInfo.LClick);
				}
			}
				
		}
		else if (GameManager.instance.pinven.stat == HandStat.Item && context.performed)
		{
			if (!GameManager.instance.uiManager.isWholeScreenUIOn)
			{
				GameManager.instance.pinven.CurHoldingItem.info?.Use();
			}
		}
	}

	//public override void Attack()
	//{
	//	//(GetActor().cast as PlayerCast).UseSkillAt(SkillSlotInfo.LClick);
	//}

	public bool ThrowRope()
	{
		camRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
		Debug.DrawRay(camRay.origin, camRay.direction * atkDist, Color.cyan, 1000f);
		if(Physics.SphereCast(camRay, 0.5f, out RaycastHit hit, atkDist, 1 << GameManager.HOOKABLELAYER, QueryTriggerInteraction.Collide))
		{
			if (hit.collider.TryGetComponent<Hookables>(out Hookables h))
			{
				h.SetRope();
				return true;
			}
		}
		return false;
	}
	
	void ResetBowStat()
	{
		//chargeEff.Reinit();
		//chargeEff.Stop();
		//loaded = false;
	}

	

	void BowDown()
	{
		//ResetBowStat();
		//(GetActor().anim as PlayerAnim).SetAttackState(((int)attackState));
		
		//animActions.ResetBowAimState();
	}

	public void SetBowStat()
	{
		//loaded = true;
		aimStart = Time.time;
	}

	public void ObtainBowRender()
	{
		animActions.eBowRend.enabled = true;
	}

	IEnumerator DelayResetCam()
	{
		yield return GameManager.instance.waitSec;
		ResetBowStat();
		GameManager.instance.SwitchTo(CamStatus.Freelook);
		//(GetActor().anim as PlayerAnim).SetAttackState(((int)attackState));
		GameManager.instance.uiManager.aimUI.On();
		ongoingResetter = null;
	}

	public override void ResetStatus()
	{
		base.ResetStatus();
		secPerCharge = initSecPerCharge;
	}
}
