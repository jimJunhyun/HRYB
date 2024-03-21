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

	
	public float targetMaxDist;
	public float targetMaxAngle;
	public float TargetMaxAngleCos
	{
		get=>Mathf.Cos(targetMaxAngle);
	}

	UnityAction updateActs;

	//float AimTime { get => Time.time - aimStart;}

	//float aimStart = 0;

	Coroutine ongoingResetter;

	internal bool clickL = false;
	internal bool clickR = false;
	public ModuleController NoClick;

	public Action<GameObject> onNextUse = default; //사용 시점에서 등장하는 이펙트 관련.
	public Action<Actor, Compose> onNextSkill = default; //자신, 스킬정보
	public Action<Vector3> onNextHit = default;

	List<StatEffID> skillEndRemovers = new List<StatEffID>(); //사용 종료시 제거되는 효과

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
	}

	public void OnAim(InputAction.CallbackContext context)
	{
		//if (GameManager.instance.pinven.stat == HandStat.Weapon)
		//{
			if (!clickL)
			{
				if ((NoClick.Paused || attackModuleStat.Paused) && !clickR)
					return;
				//Debug.LogWarning("phase : " +context.phase  + " : " + context.started + " : " + context.canceled);
				if (!clickR && context.started)
				{
					Vector3 dir = Camera.main.transform.forward;
					dir.y = 0;
					transform.rotation = Quaternion.LookRotation(dir);

					clickR = true;
					(GetActor().cast as PlayerCast).SetSkillUse(SkillSlotInfo.RClick); 
				}
				else if (clickR && context.canceled)
				{	
					clickR = false;
					(GetActor().cast as PlayerCast).ResetSkillUse(SkillSlotInfo.RClick);
				}
				
			//}
			
		}
	}

	public void OnAttack(InputAction.CallbackContext context) // 좌 우클릭
	{
		//if (GameManager.instance.pinven.stat == HandStat.Weapon)
		//{
			if (!clickR)
			{
				if ((NoClick.Paused || attackModuleStat.Paused) && !clickL)
					return;
				if (context.started && !clickL)
				{
					Vector3 dir = Camera.main.transform.forward;
					dir.y = 0;
					transform.rotation = Quaternion.LookRotation(dir);
					clickL = true;
					(GetActor().cast as PlayerCast).SetSkillUse(SkillSlotInfo.LClick);
				}
				else if (context.canceled && clickL)
				{
					clickL = false;
					(GetActor().cast as PlayerCast).ResetSkillUse(SkillSlotInfo.LClick);
				}
			}
				
		//}
		//else if (GameManager.instance.pinven.stat == HandStat.Item && context.performed)
		//{
		//	if (!GameManager.instance.uiManager.isWholeScreenUIOn)
		//	{
		//		GameManager.instance.pinven.CurHoldingItem.info?.Use();
		//	}
		//}
	}

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
		//aimStart = Time.time;
	}

	//public void ObtainBowRender()
	//{
	//	animActions.eBowRend.enabled = true;
	//}

	IEnumerator DelayResetCam()
	{
		yield return GameManager.instance.waitSec;
		ResetBowStat();
		CameraManager.instance.SwitchTo(CamStatus.Freelook);
		//(GetActor().anim as PlayerAnim).SetAttackState(((int)attackState));
		GameManager.instance.uiManager.aimUI.On();
		ongoingResetter = null;
	}

	public override void ResetStatus()
	{
		base.ResetStatus();
		secPerCharge = initSecPerCharge;
	}

	public void AddRemoveCall(StatEffID id)
	{
		skillEndRemovers.Add(id);
	}

	public void HandleRemoveCall()
	{
		for (int i = 0; i < skillEndRemovers.Count; i++)
		{
			GetActor().life.RemoveAllStatEff(skillEndRemovers[i], 1);
		}

		skillEndRemovers.Clear();
	}
}
