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
			(GetActor().anim as PlayerAnim).SetAimSpeed(base.prepMod);
		}
	}

	public float shakeFrom;
	public float maxChargeTime;

	Transform shootPos;
	Ray camRay;
	public Transform ShootPos { get; protected set;}
	float curCharge;

	bool shaking = false;

	bool atked = false;


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
	}

	private void Start()
	{
		BowDown();
	}

	private void Update()
	{
		if (loaded && attackState == AttackStates.Prepare)
		{
			
			Charge();
		}
		else if (shaking)
		{
			shaking = false;
			GameManager.instance.UnShakeCam();
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
				GameManager.instance.uiManager.aimUI.On();
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
		if(GameManager.instance.pinven.stat == HandStat.Weapon)
		{
			if (context.started && loaded && curCharge > 0.1f)
			{
				atked = true;
				GetActor().anim.SetAttackTrigger();
			}
		}
		else if(GameManager.instance.pinven.stat == HandStat.Item)
		{
			if (context.started)
			{
				GameManager.instance.pinven.CurHoldingItem.info?.Use();
			}
		}
	}

	public override void Attack()
	{
		Debug.Log($"{curCharge} 파워로 발사.");
		attackState = AttackStates.Trigger;

		Arrow r = PoolManager.GetObject("ArrowTemp", shootPos.position, shootPos.forward).GetComponent<Arrow>();
		r.SetInfo(damage, EffSpeed, isDirect);
		r.Shoot(curCharge);

		attackState = AttackStates.Prepare;
		(GetActor().anim as PlayerAnim).SetAttackState(((int)attackState));
		ResetBowStat();
		atked = false;
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
		
		curCharge += chargePerSec / atkGap * Time.deltaTime;
		
		curCharge = Mathf.Clamp(curCharge, 0, atkDist);

		if (AimTime >= maxChargeTime)
		{
			atked = true;
			GetActor().anim.SetAttackTrigger();
		}

		if (curCharge == atkDist && AimTime >= shakeFrom)
		{
			if (!shaking)
			{
				shaking = true;
				GameManager.instance.ShakeCam();
			}
			
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
		GameManager.instance.uiManager.aimUI.Off();
		animActions.ResetBowAimState();
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
		GameManager.instance.uiManager.aimUI.On();
		ongoingResetter = null;
	}
}
