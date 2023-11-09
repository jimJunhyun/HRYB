using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;




public class PlayerMove : MoveModule
{
	public const float GRAVITY = 9.8f;

	CharacterController ctrl;
	
	public float spinSpd = 300f;
	public float jumpPwer = 20f;
	public float sneakPower = 3f;

	public float slipThreshold = 45f;
	public float slipPower = 4f;

	public float lockOnDist = 15f;

	public float angleXMin;
	public float angleXMax;

	public bool jumpable;
	public bool rollable;

	public bool onAir
	{
		get => !ctrl.isGrounded;
	}

	float angle = 0;
	bool slip = false;

	Vector3 slipDir = Vector3.zero;

	Quaternion to;
	Camera mainCam;

	HashSet<Transform> already = new HashSet<Transform>();

	Transform[] targets;
	Transform[] prevTargets;


	Transform target;
	bool isLocked = false;

	public Vector3 MoveDirCalced
	{
		get
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

	public Vector3 Velocity
	{
		get => ctrl.velocity;
	}



	Transform middle;

	private void Awake()
	{
		ctrl = GetComponent<CharacterController>();
		mainCam = Camera.main;
		middle = transform.Find("Middle");
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

	private void FixedUpdate()
	{
		Move();
	}

	public override void Move()
	{
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
		ctrl.Move( (forceDir + (dir) - (Vector3.up * GRAVITY)) * Time.deltaTime);
		GetActor().anim.SetIdleState(idling);
	}

	public void Move(InputAction.CallbackContext context)
	{
		Vector2 inp = context.ReadValue<Vector2>();
		moveDir = new Vector3(inp.x, moveDir.y, inp.y);

		if (target != null && (target.position - transform.position).sqrMagnitude >= lockOnDist * lockOnDist)
		{
			ResetTargets();
		}
	}

	public void Run(InputAction.CallbackContext context)
	{
		if(moveStat != MoveStates.Sit)
		{
			if (context.started)
			{
				moveStat = MoveStates.Run;
				GetActor().anim.SetMoveState(((int)moveStat));
			}
			if (context.canceled)
			{
				moveStat = MoveStates.Walk;
				GetActor().anim.SetMoveState(((int)moveStat));
			}
		}
		

	}

	public void Crouch(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			if(moveStat == MoveStates.Sit)
			{
				moveStat = MoveStates.Walk;
				GetActor().anim.SetMoveState(((int)moveStat));
			}
			else
			{
				moveStat = MoveStates.Sit;
				GetActor().anim.SetMoveState(((int)moveStat));
			}
		}
	}

	public void Jump(InputAction.CallbackContext context)
	{
		if (jumpable && context.performed && ctrl.isGrounded && !slip)
		{
			forceDir.y += jumpPwer;
			(GetActor().anim as PlayerAnim).SetJumpTrigger();
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
			Collider[] c = Physics.OverlapSphere(transform.position, lockOnDist, ~(1 << 7 | 1 << 11));
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

	
}
