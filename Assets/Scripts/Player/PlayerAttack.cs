using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerAttack : AttackModule
{
	public float chargePerSec;

	public float ChargePerSec { get => chargePerSec;}
	public override float prepMod 
	{
		get => base.prepMod; 
		set
		{
			base.prepMod = value;
			(GetActor().anim as PlayerAnim).SetAimSpeed(value);
		}
	}

	public float shakeFrom;
	public float maxChargeTime;

	Transform shootPos;
	public Transform ShootPos { get; protected set;}
	float curCharge;

	bool shaking = false;

	bool atked = false;


	float AimTime { get => Time.time - aimStart;}

	float aimStart = 0;

	bool loaded = false;

	Coroutine ongoingResetter;

	private void Awake()
	{
		shootPos = GameObject.Find("ShootPos").transform;
		curCharge = 0;
	}

	private void Update()
	{
		if (loaded && attackState == AttackStates.Prepare)
		{
			
			Charge();
		}
		if(GameManager.instance.pinven.stat != HandStat.Weapon)
		{
			BowDown();
		}
	}

	public void OnAim(InputAction.CallbackContext context)
	{
		if(GameManager.instance.pinven.stat == HandStat.Weapon)
		{
			if (context.performed && !loaded)
			{
				GameManager.instance.SwitchTo(CamStatus.Aim);
				attackState = AttackStates.Prepare;
				(GetActor().anim as PlayerAnim).SetAttackState(((int)attackState));
			}
			if (context.canceled)
			{
				if (atked)
				{
					if (ongoingResetter == null)
					{
						ongoingResetter = StartCoroutine(DelayResetCam());
					}
				}
				else
				{
					BowDown();
				}
			}
		}
	}

	public void OnAttack(InputAction.CallbackContext context)
	{
		if (context.started && loaded && curCharge > 0.1f)
		{
			atked = true;
			GetActor().anim.SetAttackTrigger();
		}
	}

	public override void Attack()
	{
		Debug.Log($"{curCharge} 파워로 발사.");
		attackState = AttackStates.Trigger;
		(GetActor().anim as PlayerAnim).SetAttackState(((int)attackState));

		Arrow r = PoolManager.GetObject("ArrowTemp", shootPos.position, shootPos.forward).GetComponent<Arrow>();
		r.SetInfo(damage, EffSpeed, isDirect);
		r.Shoot(curCharge);

		attackState = AttackStates.Prepare;
		(GetActor().anim as PlayerAnim).SetAttackState(((int)attackState));
		ResetBowStat();
		atked = false;
	}


	void Charge()
	{
		
		curCharge += chargePerSec / atkGap * Time.deltaTime;
		
		curCharge = Mathf.Clamp(curCharge, 0, atkDist);

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

	void BowDown()
	{
		ResetBowStat();
		GameManager.instance.SwitchTo(CamStatus.Freelook);
		attackState = AttackStates.None;
		(GetActor().anim as PlayerAnim).SetAttackState(((int)attackState));
	}

	public void SetBowStat()
	{
		loaded = true;
		aimStart = Time.time;
	}

	IEnumerator DelayResetCam()
	{
		yield return new WaitUntil(()=> !atked);
		yield return GameManager.instance.waitSec;
		ResetBowStat();
		GameManager.instance.SwitchTo(CamStatus.Freelook);
		attackState = AttackStates.None;
		(GetActor().anim as PlayerAnim).SetAttackState(((int)attackState));
		ongoingResetter = null;
	}
}
