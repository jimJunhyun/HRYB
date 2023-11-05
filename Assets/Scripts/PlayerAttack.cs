using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerAttack : AttackModule
{
	public float chargePerSec;
	public float maxChargeAmt;

	public float ChargePerSec { get => chargePerSec * prepMod;}

	public float shakeFrom;
	public float maxChargeTime;

	Transform shootPos;
	public Transform ShootPos { get; protected set;}
	float curCharge;

	bool shaking = false;

	bool aiming = false;


	float AimTime { get => Time.time - aimStart;}
	float CurGap { get => Time.time - prevShot;}

	float aimStart = 0;
	float prevShot = 0;

	bool loaded = true;

	

	private readonly int bowOnHash = Animator.StringToHash("BowOn");
	private readonly int aimHash = Animator.StringToHash("Aim");

	private readonly int knifeOnHash = Animator.StringToHash("KnifeOn");

	Animator anim;

	private void Awake()
	{
		anim = GetComponentInChildren<Animator>();
		shootPos = GameObject.Find("ShootPos").transform;
		curCharge = 0;
	}

	private void Update()
	{
		if (loaded && attackState == AttackStates.Prepare)
		{
			curCharge += chargePerSec * prepMod * Time.deltaTime;
			curCharge = Mathf.Clamp(curCharge, 0, maxChargeAmt);

			if (AimTime >= maxChargeTime)
			{
				Attack();
			}

			if (AimTime >= shakeFrom && !shaking)
			{
				shaking = true;
				GameManager.instance.ShakeCam();
			}
			if(AimTime <= shakeFrom && shaking)
			{
				shaking = false;
				GameManager.instance.UnShakeCam();
			}
		}
	}

	public void OnAim(InputAction.CallbackContext context)
	{
		if (context.performed && !loaded)
		{
			GameManager.instance.SwitchTo(CamStatus.Aim);
			attackState = AttackStates.Prepare;
			StartCoroutine(DelayReShoot(atkGap));
			aiming = true;
		}
		if (context.canceled)
		{
			GameManager.instance.SwitchTo(CamStatus.Freelook);
			attackState = AttackStates.None;

			ResetBowStat();
			aiming = false;
		}
	}

	public void OnAttack(InputAction.CallbackContext context)
	{
		if (context.started && loaded && curCharge > 0.1f)	
		{
			Attack();
		}
	}

	public override void Attack()
	{
		Debug.Log($"{curCharge} 파워로 발사.");
		attackState = AttackStates.Trigger;
		GetActor().anim.SetAttackTrigger();

		Arrow r = PoolManager.GetObject("ArrowTemp", shootPos.position, shootPos.forward).GetComponent<Arrow>();
		r.SetInfo(damage, EffSpeed, isDirect);
		r.Shoot(curCharge);

		attackState = AttackStates.Prepare;
		ResetBowStat();
	}

	void ResetBowStat()
	{
		curCharge = 0;
		loaded = false;
	}

	IEnumerator DelayReShoot(float gap)
	{
		yield return new WaitForSeconds(gap);
		loaded = true;
	}
}
