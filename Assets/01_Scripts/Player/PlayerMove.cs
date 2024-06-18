using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System;
using UnityEngine.Android;



public class PlayerMove : MoveModule
{
	const string JUMPSOUNDCLIP = "DoJump";

	internal CharacterController ctrl;

	public float climbSpeed = 7f;
	public float climbDistance = 0.7f;

	public float spinSpd = 300f;
	public float jumpPwer = 20f;
	public float jumpGap = 0.5f;
	public float sneakPower = 3f;

	public float slipThreshold = 45f;
	public float climbThreshold = 85f;
	public float slipPower = 4f;

	public float lockOnDist = 15f;

	public float nearRefreshDist = 3f;
	public float nearRefreshTime = 5f;

	public float angleXMin;
	public float angleXMax;

	public bool jumpable;
	public bool rollable;
	public bool climbable;

	[Header("Avoid")]
	public float _avoidTime = 0.3f;
	public bool _isAvoid = false;

	bool AvoidSuccess = false;


	public override Vector3 forceDir
	{
		get
		{
			return fDir;
		}
		set
		{
			fDir = value;
			if (fDir.y > 0)
			{
				forced = true;
			}


		}
	}

	public bool onAir
	{
		get => moveStat != MoveStates.Climb && !moveModuleStat.Paused && !isGrounded;
	}

	float angle = 0;
	bool slip = false;
	bool climbGrounded = false;

	//bool jumped = false;
	//bool jumpComplete = false;

	float prevJump = 0;

	Vector3 slipDir = Vector3.zero;

	Vector3 ropeNormal = Vector3.zero;

	Vector3 initPos;
	Vector3 prevDetectPos;
	float prevDetectTime;

	Quaternion to;

	HashSet<Transform> already = new HashSet<Transform>();

	HashSet<Actor> targetables = new HashSet<Actor>();

	HashSet<Transform> nearTargets = new HashSet<Transform>();
	HashSet<Transform> prevNearTargets = new HashSet<Transform>();

	Transform[] targets;
	Transform[] prevTargets;


	bool isLocked = false;

	Vector3 initCenter;
	float initRad;
	float initHeight;

	float _currentAvoidTime = 0.0f;
	[Header("Avoid")]
	public float AvoidTime = 4;

	public override bool idling => base.idling || moveModuleStat.Paused;

	public override bool isGrounded
	{
		get
		{
			if (Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), Vector3.down, groundThreshold, (1 << GameManager.GROUNDLAYER) | (1 << GameManager.ENEMYLAYER)))
			{
				//Debug.DrawRay(transform.position + new Vector3(0, 0.1f, 0), Vector3.down * groundThreshold, Color.cyan, 1000f);
				return true;
			}
			//Debug.DrawRay(transform.position + new Vector3(0, 0.1f, 0), Vector3.down * groundThreshold, Color.red, 1000f);
			return false;
		}
	}

	RaycastHit hitCache;

	public ModuleController NoInput = new ModuleController(false);

	PlayerAttack pAttack;



	public Vector3 MoveDirCalced
	{
		get
		{
			if (moveStat == MoveStates.Climb)
			{
				return moveDir * Speed;
			}
			else
			{
				switch (CameraManager.instance.curCamStat)
				{
					case CamStatus.Aim:
					case CamStatus.Freelook:
						return ConvertToCamFront(moveDir) * Speed;
					case CamStatus.Locked:
						return transform.rotation * moveDir * Speed;
					default:
						return Vector3.zero;
				}
			}

		}
	}

	public Vector3 MoveDirUncalced
	{
		get
		{
			if (moveStat == MoveStates.Climb)
			{
				return moveDir * speed;
			}
			else
			{
				switch (CameraManager.instance.curCamStat)
				{
					case CamStatus.Aim:
					case CamStatus.Freelook:
						return ConvertToCamFront(moveDir) * speed;
					case CamStatus.Locked:
						return /*transform.rotation **/ moveDir * speed;
					default:
						return Vector3.zero;
				}
			}

		}
	}

	public Vector3 MoveDirWithForce
	{
		get => MoveDirUncalced + forceDir;
	}

	public override MoveStates moveStat
	{
		get => base.moveStat;
		protected set
		{
			if (value == MoveStates.Climb)
			{
				curStat = MoveStates.Climb;
				speed = climbSpeed;
			}
			else if (value == MoveStates.Sit)
			{
				if (moveStat != MoveStates.Climb)
				{
					curStat = MoveStates.Sit;
					speed = crouchSpeed.MaxValue;
				}
			}
			else
			{
				base.moveStat = value;
			}
		}
	}

	Transform middle;

	public override void Awake()
	{
		base.Awake();
		ctrl = GetComponent<CharacterController>();
		middle = transform.Find("Middle");
		initPos = transform.position;

		initCenter = ctrl.center;
		initRad = ctrl.radius;
		initHeight = ctrl.height;
	}

	private void Start()
	{
		pAttack = GetActor().atk as PlayerAttack;
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (hit.point.y <= middle.position.y)
		{
			angle = Mathf.Acos(Vector3.Dot(hit.normal, transform.up) / (hit.normal.magnitude * transform.up.magnitude)) * Mathf.Rad2Deg;

			if (angle >= slipThreshold)
			{
				slip = true;
				Vector3 c = Vector3.Cross(Vector3.up, hit.normal);
				//Debug.DrawRay(transform.position, c, Color.cyan, 1000f);
				Vector3 u = Vector3.Cross(c, hit.normal);
				//Debug.DrawRay(transform.position, u, Color.red, 1000f);
				slipDir = u * slipPower;
			}
			else
			{
				slip = false;
				slipDir = Vector3.zero;
			}
		}


	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == GameManager.TRIGGERLAYER)
		{
			GameManager.instance.qManager.InvokeOnChanged(CompletionAct.MoveTo, other.name);
		}
	}

	public void PlayerTeleport(Vector3 vec)
	{
		GameManager.instance.pinp.DeactivateInput();
		GetActor().anim.ResetStatus();
		moveDir = Vector3.zero;
		forceDir = Vector3.zero;
		transform.position = vec;
		GameManager.instance.pinp.ActivateInput();
		already.Clear();
		ResetCharacterController();
	}

	public void ResetCharacterController()
	{
		ctrl.height = initHeight;
		ctrl.radius = initRad;
		ctrl.center = initCenter;
	}

	private void Update()
	{
		//if (Input.GetKeyDown(KeyCode.Slash))
		//{
		//	jumpable = true;
		//}
		//if (Input.GetKeyDown(KeyCode.Backslash))
		//{
		//	Debug.Log("TELEPORT CHEAT");
		//	GameManager.instance.pinp.DeactivateInput();
		//	GetActor().anim.ResetStatus();
		//	moveDir = Vector3.zero;
		//	forceDir = Vector3.zero;
		//	transform.position = GameManager.instance.outCaveTmp.position;
		//	GameManager.instance.pinp.ActivateInput();
		//	already.Clear();
		//	ctrl.height = 2;
		//	ctrl.radius = 0.5f;
		//	ctrl.center = Vector3.up;
		//}
		_currentAvoidTime += Time.deltaTime;
		if (Time.time - prevDetectTime > nearRefreshTime)
		{
			DoTargetDetection();
			DoNearDetection();
		}
	}


	public override void FixedUpdate()
	{
		base.FixedUpdate();
		if (moveStat == MoveStates.Climb)
		{
			CalcClimbState();
		}
	}

	public override void Move()
	{
		if (moveStat != MoveStates.Climb)
		{
			GravityCalc();
			ForceCalc();
			SlipCalc();
			ctrl.Move((forceDir) * Time.fixedDeltaTime);
		}
		GetActor().anim.SetIdleState(idling);

		if (moveModuleStat.Paused)
		{
			//Debug.LogError("Pause");
			return;
		}
		if (moveStat != MoveStates.Climb)
		{
			if (Physics.SphereCast(middle.position, 0.5f, transform.forward, out hitCache, climbDistance, (1 << GameManager.CLIMBABLELAYER)) && moveDir.z > 0)
			{
				Debug.Log("등반");
				ropeNormal = hitCache.normal;
				SetClimb();
			}



			if (isLocked && (self.atk.target) == null)
			{
				ResetTargets();
			}
			switch (CameraManager.instance.curCamStat)
			{
				case CamStatus.Freelook:
					{
						Vector3 vec = MoveDirCalced;

						if (vec.sqrMagnitude > 0.01f)
						{
							to = Quaternion.LookRotation(vec, Vector3.up);
							RotateTo();
						}
						PlayerControllerMove(vec);
					}

					break;
				case CamStatus.Locked:
					{
						if ((self.atk.target) == null)
							return;
						Vector3 vec = GetDir((self.atk.target));
						to = Quaternion.LookRotation(vec, Vector3.up);
						if (to != Quaternion.identity)
						{
							RotateTo();
						}
						PlayerControllerMove(MoveDirCalced);
					}

					break;
				case CamStatus.Aim:
					{
						Vector3 vec = MoveDirCalced;

						PlayerControllerMove(vec);
					}
					break;
				default:
					break;
			}
		}
		else
		{

			if (climbGrounded && moveDir.y < 0)
			{
				Debug.Log("착지");
				ResetClimb();
			}

			if (!climbGrounded && !Physics.SphereCast(transform.position, 0.5f, transform.forward, out hitCache, climbDistance, (1 << GameManager.CLIMBABLELAYER)) && moveDir.y > 0)
			{
				Debug.Log("등반완");
				PlayerControllerMove(-ropeNormal * climbSpeed);
				if (Physics.Raycast(transform.position, Vector3.down, 2f, ~(1 << GameManager.PLAYERLAYER)))
				{
					ResetClimb();
				}
			}
			else
			{
				PlayerControllerMove(MoveDirCalced);
			}
		}


	}

	public void CalcClimbState()
	{
		if (moveStat == MoveStates.Climb)
		{
			Debug.DrawRay(middle.position, Vector3.down * 1.5f, Color.cyan, 1000f);
			if (Physics.Raycast(transform.position, Vector3.down, 0.5f, ~(1 << GameManager.PLAYERLAYER)))
			{
				Debug.Log("GND");
				climbGrounded = true;
			}
			else
			{
				climbGrounded = false;
			}
		}


	}

	void SlipCalc()
	{
		if (slip)
		{
			forceDir += slipDir * Time.deltaTime;
		}
	}

	Vector3 GetDir(Transform to)
	{
		Vector3 v = to.position - transform.position;
		v.y = 0;
		return v;
	}

	Vector3 ConvertToCamFront(Vector3 original)
	{
		Vector3 rot = CameraManager.instance.MainCam.transform.eulerAngles;
		rot.x = 0;
		Vector3 v = Quaternion.Euler(rot) * original;
		return v;
	}

	public void RotateTo()
	{
		transform.rotation = Quaternion.RotateTowards(transform.rotation, to, spinSpd);
	}

	public void PlayerControllerMove(Vector3 dir)
	{
		try
		{
			if (dir.sqrMagnitude > 0.1f && ctrl.isGrounded)
			{
				/*
				switch (moveStat)
				{
					case MoveStates.Run:
						if (GameManager.GetLayerName(transform.position, GameManager.instance.terrain)
						    .Contains("Grass"))
						{
							
							GameManager.instance.audioPlayer.PlayGlobal("GrassRun");
						}
						else
						{
							GameManager.instance.audioPlayer.PlayGlobal("StoneRun");
						}

						break;
					case MoveStates.Walk:
					case MoveStates.Sit:
						if (GameManager.GetLayerName(transform.position, GameManager.instance.terrain)
						    .Contains("Grass"))
						{
							GameManager.instance.audioPlayer.PlayGlobal("GrassWalk");
						}
						else
						{
							GameManager.instance.audioPlayer.PlayGlobal("StoneWalk");
						}

						break;
					case MoveStates.Climb:
						break;
				}
				*/
			}
			else if (GameManager.instance.audioPlayer.IsPlaying)
			{
				GameManager.instance.audioPlayer.StopGlobal(); //추후 수정 필요%%%%%%%%%%%%
			}
		}
		catch
		{
			Debug.LogWarning("사운드 혐오");
		}

		if (IsActualGrounded)
		{
			dir -= Vector3.up * GameManager.GRAVITY;
		}

		if ((self.atk.target) != null && ((self.atk.target).position - transform.position).sqrMagnitude >= lockOnDist * lockOnDist)
		{
			ResetTargets();
		}

		if ((transform.position - prevDetectPos).sqrMagnitude > nearRefreshDist * nearRefreshDist)
		{
			DoTargetDetection();
			DoNearDetection();
		}

		//if (pAttack.target != null && (transform.position - pAttack.target.position).sqrMagnitude > pAttack.targetMaxDist * pAttack.targetMaxDist)
		//{
		//	pAttack.target = null;
		//}

		ctrl.Move((dir) * Time.fixedDeltaTime);




	}
	int t = 0;

	public void Move(InputAction.CallbackContext context)
	{
		if (!NoInput.Paused)
		{
			Vector2 inp = context.ReadValue<Vector2>();

			//Debug.LogError($"move context {t++}");
			if (moveStat != MoveStates.Climb)
			{
				moveDir = new Vector3(inp.x, moveDir.y, inp.y);
			}
			else
			{
				moveDir = new Vector3(0, inp.y, 0);
			}


		}

	}

	public void Wheel(InputAction.CallbackContext context)
	{
		CameraManager.instance.Wheel(context);
	}

	public void Run(InputAction.CallbackContext context)
	{
		if (moveStat != MoveStates.Sit && moveStat != MoveStates.Climb)
		{
			if (context.started)
			{
				if (moveStat == MoveStates.Run)
				{
					moveStat = MoveStates.Walk;
				}
				else if (moveStat == MoveStates.Walk)
				{
					moveStat = MoveStates.Run;
				}

			}
			GetActor().anim.SetMoveState(((int)moveStat));
		}


	}

	public void Crouch(InputAction.CallbackContext context)
	{
		if (context.started && moveStat != MoveStates.Climb)
		{
			if (moveStat == MoveStates.Sit)
			{
				Debug.DrawRay(middle.position, Vector3.up * ctrl.height, Color.green, 1000f);
				if (!Physics.Raycast(middle.position, Vector3.up, ctrl.height, ~(1 << GameManager.PLAYERLAYER), QueryTriggerInteraction.Ignore))
				{
					moveStat = MoveStates.Walk;
					ctrl.height *= 2f;
					ctrl.center *= 2f;
				}

			}
			else
			{
				ctrl.height *= 0.5f;
				ctrl.center *= 0.5f;
				moveStat = MoveStates.Sit;
			}
			GetActor().anim.SetMoveState(((int)moveStat));
		}
	}

	public void Jump(InputAction.CallbackContext context)
	{
		if (!NoInput.Paused && !moveModuleStat.Paused)
		{
			if (context.performed && jumpable)
			{
				if (moveStat == MoveStates.Climb)
				{
					forceDir += Vector3.up * (jumpPwer / climbSpeed);
					Vector3 ropeJumpDir = (ropeNormal + Vector3.up).normalized;
					forceDir += ropeJumpDir * jumpPwer;
					ResetClimb();
				}
				else
				{
					if (ctrl.isGrounded && Time.time - prevJump >= jumpGap)
					{
						prevJump = Time.time;
						forceDir += Vector3.up * jumpPwer;
						(GetActor().anim as PlayerAnim).SetJumpTrigger();
						GameManager.instance.audioPlayer.PlayPoint(JUMPSOUNDCLIP, transform.position);
					}
				}
			}
		}


	}

	public void Turn(InputAction.CallbackContext context)
	{
		if (CameraManager.instance.curCamStat == CamStatus.Aim)
		{
			Vector2 inp = context.ReadValue<Vector2>();
			transform.Rotate(Vector3.up * inp.x * spinSpd * Time.deltaTime);
			CameraManager.instance.aimCam.transform.Rotate(Vector3.left * inp.y * spinSpd * Time.deltaTime);
			CameraManager.instance.aimCam.transform.eulerAngles = new Vector3(GameManager.ClampAngle(CameraManager.instance.aimCam.transform.eulerAngles.x, angleXMin, angleXMax)
				, CameraManager.instance.aimCam.transform.eulerAngles.y
				, CameraManager.instance.aimCam.transform.eulerAngles.z);
		}

		if (CameraManager.instance.curCamStat == CamStatus.Freelook)
		{
			if (targetables.Count == 0)
			{
				DoTargetDetection();
			}
			if (targetables.Count > 0)
			{
				SetNearestEnemy();
			}

		}
	}

	//float _targetTime = 0;
	//private void LateUpdate()
	//{
	//	if(self.atk.target != null && CameraManager.instance.curCamStat == CamStatus.Freelook)
	//	{
	//		_targetTime += Time.deltaTime;
	//	}
	//
	//	if(_targetTime > 2f && CameraManager.instance.curCamStat == CamStatus.Freelook)
	//	{
	//		_targetTime = 0;
	//		CameraManager.instance.SwitchTo(CamStatus.Locked);
	//	}
	//}

	public void Lock(InputAction.CallbackContext context)
	{
		if (!NoInput.Paused)
		{
			if (context.started)
			{
				Collider[] c = Physics.OverlapSphere(transform.position, lockOnDist, 1 << GameManager.ENEMYLAYER);
				if (c.Length > 0)
				{
					prevTargets = targets;
					targets = c.Select(item => item.transform).OrderBy(item => (item.position - transform.position).sqrMagnitude).ToArray();

					if (prevTargets != null && targets != null)
					{
						IEnumerable<Transform> t = prevTargets.Except(targets);
						if (t.Any())
						{
							Transform[] removes = t.ToArray();
							for (int i = 0; i < removes.Length; i++)
							{
								already.Remove(removes[i]);
							}
						}
					}
				}

				if (targets != null)
				{
					bool found = false;
					if ((self.atk.target) != null)
					{
						already.Add((self.atk.target));
					}

					for (int i = 0; i < targets.Length; i++)
					{
						if (!already.Contains(targets[i]))
						{
							(self.atk.target) = targets[i];
							pAttack.target = (self.atk.target).Find("Middle");

							CameraManager.instance.SwitchTo(CamStatus.Locked);

							isLocked = true;
							found = true;
							break;
						}
					}
					if (!found)
					{
						ResetTargets();
					}
				}
				else
				{
					ResetTargets();
				}
			}
		}

	}

	public void Roll(InputAction.CallbackContext context)
	{
		//if (!NoInput.Paused && !moveModuleStat.Paused)
		//{
		//	if (context.started && moveStat != MoveStates.Climb && rollable && IsActualGrounded)
		//	{
		//		GetActor().life.isImmune = true;
		//		GameManager.instance.pinp.DeactivateInput();
		//		moveDir = Vector3.forward;
		//		to = Quaternion.LookRotation(moveDir, Vector3.up);
		//		RotateTo();
		//		(GetActor().anim as PlayerAnim).SetRollTrigger();
		//		ctrl.height *= 0.5f;
		//		ctrl.radius *= 0.5f;
		//		if (GetActor().move.moveStat != MoveStates.Sit)
		//			ctrl.center *= 0.5f;
		//	}
		//}


	}

	public void DoNearDetection()
	{
		prevNearTargets = new HashSet<Transform>(nearTargets);
		nearTargets.Clear();
		Collider[] c = Physics.OverlapSphere(transform.position, GameManager.NEARTHRESHOLD, ~((1 << GameManager.PLAYERATTACKLAYER) | (1 << GameManager.PLAYERLAYER | 1 << GameManager.GROUNDLAYER)));
		for (int i = 0; i < c.Length; i++)
		{
			nearTargets.Add(c[i].transform);
		}

		HashSet<Transform> nearEnters = new HashSet<Transform>(nearTargets);
		nearEnters.ExceptWith(prevNearTargets);

		foreach (var item in nearEnters)
		{
			GameManager.instance.qManager.InvokeOnChanged(CompletionAct.GoNear, item.name);
			GameManager.instance.qManager.InvokeOnChanged(CompletionAct.RemainNear, item.name);
		}

		HashSet<Transform> nearExits = new HashSet<Transform>(prevNearTargets);
		nearExits.ExceptWith(nearTargets);

		foreach (var item in nearExits)
		{
			if (item != null)
			{
				GameManager.instance.qManager.InvokeOnChanged(CompletionAct.RemainNear, item.name, -1);
			}
		}
	}

	public void DoTargetDetection()
	{
		targetables.Clear();
		Collider[] c = Physics.OverlapSphere(transform.position, pAttack.targetMaxDist, ~((1 << GameManager.PLAYERATTACKLAYER | 1 << GameManager.PLAYERLAYER | 1 << GameManager.GROUNDLAYER)));
		//Debug.Log("HIT OF  + " + c.Length);
		for (int i = 0; i < c.Length; i++)
		{
			if (c[i].TryGetComponent<Actor>(out Actor actor))
			{
				if (actor != GetActor() && actor.life.yy.white.Value > 0)
				{
					targetables.Add(actor);
				}
			}
		}

		prevDetectPos = transform.position;
		prevDetectTime = Time.time;

		//Debug.Log("DETECTED " + targetables.Count);
		SetNearestEnemy();
	}

	public void SetNearestEnemy()
	{
		pAttack.target = null;

		float nearestDist = float.MaxValue;

		foreach (var item in targetables)
		{
			if (!item)
				continue;
			Vector3 distVec = (item.transform.position - transform.position);
			if (Vector3.Dot(distVec.normalized, Camera.main.transform.forward) > pAttack.TargetMaxAngleCos)
			{
				if (distVec.sqrMagnitude < nearestDist)
				{
					nearestDist = distVec.sqrMagnitude;
					pAttack.target = item.transform;
					Debug.Log("ATARETEGET");
				}

			}

		}
	}

	public float GetSneakDist()
	{
		if (moveStat == MoveStates.Sit)
		{
			return sneakPower;
		}
		return 0;
	}


	void ResetTargets()
	{
		(self.atk.target) = null;
		already.Clear();

		CameraManager.instance.SwitchTo(CamStatus.Freelook);

		isLocked = false;
	}

	void ResetClimb()
	{
		moveStat = MoveStates.Walk;
		moveDir = Vector3.zero;
		GetActor().anim.SetMoveState(((int)moveStat));
	}

	void SetClimb()
	{
		moveStat = MoveStates.Climb;
		forceDir = Vector3.zero;
		slipDir = Vector3.zero;
		moveDir = Vector3.zero;
		GetActor().anim.SetMoveState(((int)moveStat));
	}

	public override void ResetStatus()
	{
		base.ResetStatus();
		//transform.position = initPos;
		GameManager.instance.pinp.ActivateInput();
		already.Clear();
		ResetCharacterController();

	}

	public void PlayerGoHome()
	{
		PlayerTeleport(new Vector3(0, 0, 0));
	}

	public void AvoidPlayer()
	{
		if(_currentAvoidTime < AvoidTime)
		{
			return;
		}

		_currentAvoidTime = 0;

		GameManager.instance.DisableCtrl();
		self.anim.SetBoolModify("Avoid", true);
		Debug.LogError("Move ment : " + moveDir);
		Vector3 dir = moveDir;
		if (dir == Vector3.zero)
		{
			dir = new Vector3(0, 0, -1);
			//self.anim.SetIntigerModify("MoveX", -1);
		}
		self.anim.SetFloatModify("FlipX", (int)dir.x);
		self.anim.SetFloatModify("FlipY", (int)dir.z);

		if (GameManager.instance.camManager.curCamStat == CamStatus.Freelook)
		{
			Vector3 dir2 = Camera.main.transform.forward;
			dir2.y = 0;
			self.transform.rotation = Quaternion.LookRotation(dir2);

		}

		dir = ConvertToCamFront(dir);
		dir = new Vector3(dir.x * 12, 3, dir.z * 12);

		forceDir += dir;

		StartCoroutine(AvoidDelay());

		self.life._hitEvent += Hitting;
	}

	IEnumerator AvoidDelay()
	{
		_isAvoid = true;
		yield return new WaitForSeconds(_avoidTime);
		_isAvoid = false;
		//forceDir = Vector3.zero;
	}

	public void Hitting()
	{
		self.life._hitEvent -= Hitting;
		self.anim.SetBoolModify("Avoid", false);
	}

}
