using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public enum AttackStates
{
	None,
	Charge,

}

public class PlayerAttack : AttackModule
{
	protected float secPerCharge;
	public float initSecPerCharge;

	public float SecPerCharge { get => secPerCharge; }

	Transform shootPos;
	VisualEffect chargeEff;
	Ray camRay;
	public Transform ShootPos { get; protected set;}
	int curCharge;


	float AimTime { get => Time.time - aimStart;}

	float aimStart = 0;

	bool loaded = false;

	Coroutine ongoingResetter;

	PlayerAnimActions animActions;

	private void Awake()
	{
		shootPos = GameObject.Find("ShootPos").transform;

		animActions = GetComponentInChildren<PlayerAnimActions>();
		curCharge = 0;

		chargeEff = shootPos.GetComponentInChildren<VisualEffect>();
		chargeEff.Stop();
	}

	private void Start()
	{
		BowDown();
	}

	private void Update()
	{
		if (loaded && attackState == AttackStates.Charge)
		{
			Charge();
		}
	}

	public void OnAim(InputAction.CallbackContext context)
	{
		if(GameManager.instance.pinven.stat == HandStat.Weapon)
		{
			if (context.performed && !loaded)
			{
				GameManager.instance.SwitchTo(CamStatus.Aim);
				attackState = AttackStates.Charge;
				(GetActor().anim as PlayerAnim).SetAttackState(((int)attackState));
				GameManager.instance.uiManager.aimUI.On();
				chargeEff.Play();
			}
			if (context.canceled)
			{
				Attack();
				BowDown();
				
			}
		}
	}

	public void OnAttack(InputAction.CallbackContext context)
	{
		if(GameManager.instance.pinven.stat == HandStat.Weapon)
		{
			if (context.started && loaded && attackState == AttackStates.None)
			{
				GetActor().anim.SetAttackTrigger();
			}
		}
		else if(GameManager.instance.pinven.stat == HandStat.Item)
		{
			if (context.started && !GameManager.instance.uiManager.isWholeScreenUIOn)
			{
				GameManager.instance.pinven.CurHoldingItem.info?.Use();
			}
		}
	}

	public override void Attack()
	{
		Debug.Log("공격입력됨");
		(GetActor().cast as PlayerCast).UseMainSkill();
		(GetActor().cast as PlayerCast).ResetComboMain();
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


	void Charge()
	{
		if(curCharge != (int)(AimTime / secPerCharge))
		{
			curCharge = (int)(AimTime / secPerCharge);
			(GetActor().cast as PlayerCast).NextComboMain(false);
			Debug.Log("Combo Proceeded");
		}
	}

	

	void ResetBowStat()
	{
		chargeEff.Reinit();
		chargeEff.Stop();
		curCharge = 0;
		loaded = false;
	}

	void BowDown()
	{
		ResetBowStat();
		GameManager.instance.SwitchTo(CamStatus.Freelook);
		attackState = AttackStates.None;
		(GetActor().anim as PlayerAnim).SetAttackState(((int)attackState));
		GameManager.instance.uiManager.aimUI.Off();
		animActions.ResetBowAimState();
	}

	public void SetBowStat()
	{
		loaded = true;
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
		attackState = AttackStates.None;
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
