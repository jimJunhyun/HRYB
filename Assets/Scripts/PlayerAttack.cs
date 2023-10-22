using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : AttackModule
{
	

	public float chargePerSec;
	public float maxChargeAmt;

	public float shakeFrom;
	public float maxChargeTime;

	Transform shootPos;

	float curCharge;
	bool isAimed = false;

	bool shaking = false;


	float AimTime { get => Time.time - aimStart;}
	float CurGap { get => Time.time - prevShot;}

	float aimStart = 0;
	float prevShot = 0;

	bool loaded = false;

	private readonly int bowOnHash = Animator.StringToHash("BowOn");
	private readonly int aimHash = Animator.StringToHash("Aim");

	Animator anim;

	private void Awake()
	{
		anim = GetComponentInChildren<Animator>();
		shootPos = GameObject.Find("ShootPos").transform;
		curCharge = 0;
	}

	private void Update()
	{
		if (isAimed)
		{
			curCharge += chargePerSec * Time.deltaTime;
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
		if (context.performed && !isAimed)
		{
			GameManager.instance.SwitchTo(CamStatus.Aim);
			if (loaded)
			{
				isAimed = true; // 로드 상태에서만 충전이 쌓이도록?
				aimStart = Time.time;
			}
		}
		if (context.canceled)
		{
			GameManager.instance.SwitchTo(CamStatus.Freelook);
			loaded = false;
			isAimed = false;
			curCharge = 0;
		}
	}

	public void OnAttack(InputAction.CallbackContext context)
	{
		if (context.started && loaded && isAimed)	
		{
			Attack();
		}
	}

	public override void Attack()
	{
		Debug.Log($"{curCharge} 파워로 발사.");
		Arrow r = PoolManager.GetObject("ArrowTemp", shootPos.position, shootPos.forward).GetComponent<Arrow>();
		r.SetInfo(damage, effSpeed, isDirect);
		r.Shoot(curCharge);
		curCharge = 0;
		StartCoroutine(DelayReShoot(atkGap));
	}

	IEnumerator DelayReShoot(float gap)
	{
		yield return new WaitForSeconds(gap);
		prevShot = Time.time;
		aimStart = Time.time;
		loaded = true;
	}
}
