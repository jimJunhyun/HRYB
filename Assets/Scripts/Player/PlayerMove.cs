using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System;

public class PlayerMove : MoveModule
{
	public const float GRAVITY = 9.8f;
	

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


	public float angleXMin;
	public float angleXMax;

	public bool jumpable;
	public bool rollable;
	public bool climbable;

	public bool onAir
	{
		get => moveStat != MoveStates.Climb && !ctrl.isGrounded;
	}

	float angle = 0;
	bool slip = false;
	bool climbGrounded = false;

	float prevJump = 0;

	Vector3 slipDir = Vector3.zero;

	Vector3 ropeNormal = Vector3.zero;

	Vector3 initPos;

	Quaternion to;
	Camera mainCam;

	HashSet<Transform> already = new HashSet<Transform>();

	Transform[] targets;
	Transform[] prevTargets;


	Transform target;
	bool isLocked = false;

	RaycastHit hitCache;

	

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
				switch (GameManager.instance.curCamStat)
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
				switch (GameManager.instance.curCamStat)
				{
					case CamStatus.Aim:
					case CamStatus.Freelook:
						return ConvertToCamFront(moveDir) * speed;
					case CamStatus.Locked:
						return transform.rotation * moveDir * speed;
					default:
						return Vector3.zero;
				}
			}

		}
	}

	public override MoveStates moveStat 
	{
		get => base.moveStat; 
		protected set 
		{
			if(value == MoveStates.Climb)
			{
				curStat = MoveStates.Climb;
				speed = climbSpeed;
			}
			else if(value == MoveStates.Sit)
			{
				if (moveStat != MoveStates.Climb)
				{
					curStat = MoveStates.Sit;
					speed = crouchSpeed;
				}
			}
			else
			{
				base.moveStat = value;
			}
		}
	}

	Transform middle;

	private void Awake()
	{
		ctrl = GetComponent<CharacterController>();
		mainCam = Camera.main;
		middle = transform.Find("Middle");
		initPos = transform.position;
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if(hit.point.y <= middle.position.y)
		{
			

			angle = Mathf.Acos(Vector3.Dot(hit.normal, transform.up) / (hit.normal.magnitude * transform.up.magnitude)) * Mathf.Rad2Deg;
			
			if (angle >= slipThreshold)
			{
				slip = true;
				Vector3 c = Vector3.Cross(Vector3.up, hit.normal);
				Debug.DrawRay(transform.position, c, Color.cyan, 1000f);
				Vector3 u = Vector3.Cross(c, hit.normal);
				Debug.DrawRay(transform.position, u, Color.red, 1000f);
				slipDir = u * slipPower;
			}
			else
			{
				slip = false;
				slipDir = Vector3.zero;
			}
		}

		
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Slash))
		{
			jumpable = true;
		}
		if (Input.GetKeyDown(KeyCode.Backslash))
		{
			Debug.Log("TELEPORT CHEAT");
			GameManager.instance.pinp.DeactivateInput();
			GetActor().anim.ResetStatus();
			moveDir = Vector3.zero;
			forceDir = Vector3.zero;
			transform.position = GameManager.instance.outCaveTmp.position;
			GameManager.instance.pinp.ActivateInput();
			already.Clear();
			ctrl.height = 2;
			ctrl.radius = 0.5f;
			ctrl.center = Vector3.up;
		}
	}
	

	private void FixedUpdate()
	{
		Move();
		if (moveStat == MoveStates.Climb)
		{
			CalcClimbState();
		}
	}

	public override void Move()
	{
		if (moveStat != MoveStates.Climb)
		{
			if (Physics.SphereCast(middle.position, 0.5f, transform.forward, out hitCache, climbDistance, (1 << GameManager.CLIMBABLELAYER)) && moveDir.z > 0)
			{
				Debug.Log("등반");
				ropeNormal = hitCache.normal;
				SetClimb();
			}

			ForceCalc();
			SlipCalc();
			GravityCalc();
			

			if (isLocked && target == null)
			{
				ResetTargets();
			}
			switch (GameManager.instance.curCamStat)
			{
				case CamStatus.Freelook:
					{
						Vector3 vec = MoveDirCalced;

						if (vec.sqrMagnitude != 0)
						{
							to = Quaternion.LookRotation(vec, Vector3.up);
						}
						RotateTo();
						PlayerControllerMove(vec);
					}

					break;
				case CamStatus.Locked:
					{
						Vector3 vec = GetDir(target);
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
		else{

			if (climbGrounded && moveDir.y < 0)
			{
				Debug.Log("착지");
				ResetClimb();
			}

			if(!climbGrounded && !Physics.SphereCast(transform.position, 0.5f, transform.forward, out hitCache, climbDistance, (1 << GameManager.CLIMBABLELAYER)) && moveDir.y > 0)
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
		if(moveStat == MoveStates.Climb)
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

	void GravityCalc()
	{
		if (!ctrl.isGrounded)
		{
			forceDir.y -= GRAVITY * Time.deltaTime;
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
		Vector3 rot = mainCam.transform.eulerAngles;
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
		if (dir.sqrMagnitude > 0.1f && !ctrl.isGrounded)
		{
			switch (moveStat)
			{
				case MoveStates.Run:
					if(GameManager.GetLayerName(transform.position, GameManager.instance.terrain).Contains("Grass")){
						GameManager.instance.audioPlayer.PlayGlobal("GrassRun");
					}
					else
					{
						GameManager.instance.audioPlayer.PlayGlobal("StoneRun");
					}
					break;
				case MoveStates.Walk:
				case MoveStates.Sit:
					if (GameManager.GetLayerName(transform.position, GameManager.instance.terrain).Contains("Grass"))
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
		}
		else if (GameManager.instance.audioPlayer.IsPlaying)
		{
			GameManager.instance.audioPlayer.StopGlobal();
		}


		if (moveStat != MoveStates.Climb)
		{
			dir -= Vector3.up * GRAVITY;
			dir += forceDir;
		}
		ctrl.Move( (dir) * Time.deltaTime);
		GetActor().anim.SetIdleState(idling);
		
		
	}

	public void Move(InputAction.CallbackContext context)
	{
		Vector2 inp = context.ReadValue<Vector2>();
		if (moveStat != MoveStates.Climb)
		{
			moveDir = new Vector3(inp.x, moveDir.y, inp.y);
			
		}
		else
		{
			moveDir = new Vector3(0, inp.y, 0);
			
		}

		if (target != null && (target.position - transform.position).sqrMagnitude >= lockOnDist * lockOnDist)
		{
			ResetTargets();
		}
	}

	public void Run(InputAction.CallbackContext context)
	{
		if(moveStat != MoveStates.Sit && moveStat != MoveStates.Climb)
		{
			if (context.started)
			{
				moveStat = MoveStates.Run;
			}
			if (context.canceled)
			{
				moveStat = MoveStates.Walk;
			}
			GetActor().anim.SetMoveState(((int)moveStat));
		}
		

	}

	public void Crouch(InputAction.CallbackContext context)
	{
		if (context.started && moveStat != MoveStates.Climb)
		{
			if(moveStat == MoveStates.Sit)
			{
				Debug.DrawRay(middle.position, Vector3.up * ctrl.height, Color.green, 1000f);
				if(!Physics.Raycast(middle.position,Vector3.up, ctrl.height, ~(1 << GameManager.PLAYERLAYER), QueryTriggerInteraction.Ignore))
				{
					moveStat = MoveStates.Walk;
					ctrl.height *= 2f;
					ctrl.center = Vector3.up;
				}
				
			}
			else
			{
				ctrl.height *= 0.5f;
				ctrl.center = Vector3.up * 0.5f;
				moveStat = MoveStates.Sit;
			}
			GetActor().anim.SetMoveState(((int)moveStat));
		}
	}

	public void Jump(InputAction.CallbackContext context)
	{
		if (context.performed && jumpable)
		{
			if (moveStat == MoveStates.Climb)
			{
				forceDir.y += jumpPwer / climbSpeed;
				Vector3 ropeJumpDir = (ropeNormal + Vector3.up).normalized;
				forceDir += ropeJumpDir * jumpPwer;
				ResetClimb();
			}
			else
			{
				if (ctrl.isGrounded && Time.time - prevJump >= jumpGap)
				{
					prevJump = Time.time;
					forceDir.y += jumpPwer;
					(GetActor().anim as PlayerAnim).SetJumpTrigger();
				}
			}
		}
		
	}

	public void Turn(InputAction.CallbackContext context)
	{
		if(GameManager.instance.curCamStat == CamStatus.Aim)
		{
			Vector2 inp = context.ReadValue<Vector2>();
			transform.Rotate(Vector3.up * inp.x * spinSpd * Time.deltaTime);
			GameManager.instance.aimCam.transform.Rotate(Vector3.left * inp.y * spinSpd * Time.deltaTime);
			GameManager.instance.aimCam.transform.eulerAngles = new Vector3(GameManager.ClampAngle(GameManager.instance.aimCam.transform.eulerAngles.x, angleXMin, angleXMax), GameManager.instance.aimCam.transform.eulerAngles.y, GameManager.instance.aimCam.transform.eulerAngles.z);
		}
	}

	public void Lock(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			Collider[] c = Physics.OverlapSphere(transform.position, lockOnDist, ~(1 << GameManager.PLAYERLAYER | 1 << GameManager.GROUNDLAYER));
			if (c.Length > 0)
			{
				prevTargets = targets;
				targets = c.Select(item => item.transform).OrderBy(item => (item.position - transform.position).sqrMagnitude).ToArray();
				
				if(prevTargets != null && targets != null)
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

			if(targets != null)
			{
				bool found = false;
				if (target != null)
				{
					already.Add(target);
				}

				for (int i = 0; i < targets.Length; i++)
				{
					if (!already.Contains(targets[i]))
					{
						target = targets[i];

						GameManager.instance.SwitchTo(CamStatus.Locked);
						
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

	public void Roll(InputAction.CallbackContext context)
	{
		if (context.started && moveStat != MoveStates.Climb)
		{
			GetActor().life.isImmune = true;
			GameManager.instance.pinp.DeactivateInput();
			moveDir = Vector3.forward;
			to = Quaternion.LookRotation(moveDir, Vector3.up);
			RotateTo();
			(GetActor().anim as PlayerAnim).SetRollTrigger();
			ctrl.height *= 0.25f;
			ctrl.radius *= 0.5f;
			if(GetActor().move.moveStat != MoveStates.Sit)
				ctrl.center *= 0.5f;
		}
		
	}

	public float GetSneakDist()
	{
		if(moveStat == MoveStates.Sit)
		{
			return sneakPower;
		}
		return 0;
	}

	

	

	void ResetTargets()
	{
		target = null;
		already.Clear();

		GameManager.instance.SwitchTo(CamStatus.Freelook);

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
		transform.position = initPos;
		GameManager.instance.pinp.ActivateInput();
		already.Clear();
		ctrl.height = 2;
		ctrl.radius = 0.5f;
		ctrl.center = Vector3.up;
	}
}
