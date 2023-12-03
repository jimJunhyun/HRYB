using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.VFX;


public class PlayerAttack : AttackModule
{
	protected float secPerCharge;
	public float initSecPerCharge;

	public float SecPerCharge { get => secPerCharge; }

	Transform shootPos;
	//VisualEffect chargeEff;
	Ray camRay;
	public Transform ShootPos { get; protected set;}
	int curCharge;

	UnityAction updateActs;

	float AimTime { get => Time.time - aimStart;}

	float aimStart = 0;

	Coroutine ongoingResetter;

	PlayerAnimActions animActions;

	private void Awake()
	{
		shootPos = GameObject.Find("ShootPos").transform;

		animActions = GetComponentInChildren<PlayerAnimActions>();
		curCharge = 0;

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
		if(GameManager.instance.pinven.stat == HandStat.Weapon)
		{
			if (context.performed)
			{
				GameManager.instance.SwitchTo(CamStatus.Aim);
				(GetActor().anim as PlayerAnim).SetAttackState(((int)attackState));
				GameManager.instance.uiManager.aimUI.On();
				//chargeEff.Play();
			}
			if (context.canceled)
			{
				Attack();
				Disoperater(SkillSlotInfo.RClick);
				BowDown();
				
			}
		}
	}

	public void OnAttack(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			Debug.Log("공격입력됨");
			if (GameManager.instance.pinven.stat == HandStat.Weapon)
			{
				Attack();
				NextCombo(SkillSlotInfo.LClick); //###
			}
			else if (GameManager.instance.pinven.stat == HandStat.Item)
			{
				if (!GameManager.instance.uiManager.isWholeScreenUIOn)
				{
					GameManager.instance.pinven.CurHoldingItem.info?.Use();
				}
			}
		}
		
	}

	public void Disoperater(SkillSlotInfo at)
	{
		(GetActor().cast as PlayerCast).DisoperateAt(at);
	}

	public void NextCombo(SkillSlotInfo at)
	{
		(GetActor().cast as PlayerCast).NextComboAt(at);
	}

	public override void Attack()
	{
		(GetActor().cast as PlayerCast).UseSkillAt(SkillSlotInfo.LClick);
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
		curCharge = 0;
		//loaded = false;
	}

	void BowDown()
	{
		ResetBowStat();
		GameManager.instance.SwitchTo(CamStatus.Freelook);
		(GetActor().anim as PlayerAnim).SetAttackState(((int)attackState));
		GameManager.instance.uiManager.aimUI.Off();
		animActions.ResetBowAimState();
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
		(GetActor().anim as PlayerAnim).SetAttackState(((int)attackState));
		GameManager.instance.uiManager.aimUI.On();
		ongoingResetter = null;
	}

	public override void ResetStatus()
	{
		base.ResetStatus();
		secPerCharge = initSecPerCharge;
	}
}
