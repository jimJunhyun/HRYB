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


	float AimTime { get => Time.time - aimStart;}
	
	float curGap = 0;

	float aimStart = 0;

	bool loaded = false;


	private void Awake()
	{
		shootPos = GameObject.Find("ShootPos").transform;
		curCharge = 0;
	}

	private void Update()
	{
		if(!loaded && attackState == AttackStates.Prepare)
		{
			curGap += Time.deltaTime;
			if(curGap > atkGap)
			{
				loaded = true;
				curGap = 0;
			}
		}
		if (loaded && attackState == AttackStates.Prepare)
		{

			aimStart = Time.time;
			Charge();
		}
	}

	public void OnAim(InputAction.CallbackContext context)
	{
		if (context.performed && !loaded)
		{
			GameManager.instance.SwitchTo(CamStatus.Aim);
			attackState = AttackStates.Prepare;
		}
		if (context.canceled)
		{
			GameManager.instance.SwitchTo(CamStatus.Freelook);
			attackState = AttackStates.None;

			ResetBowStat();
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


	void Charge()
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
		if (AimTime <= shakeFrom && shaking)
		{
			shaking = false;
			GameManager.instance.UnShakeCam();
		}
	}

	void ResetBowStat()
	{
		curCharge = 0;
		loaded = false;
	}
}
