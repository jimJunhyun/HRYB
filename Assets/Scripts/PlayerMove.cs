using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class PlayerMove : MonoBehaviour
{
	CharacterController ctrl;
	public float speed = 4f;
	public float spinSpd = 300f;
	public float jumpPwer = 20f;

	public float slipThreshold = 45f;
	public float slipPower = 4f;

	public float lockOnDist = 15f;

	float gravityAccel = 0;
	float angle = 0;
	bool slip = false;

	Vector3 slipDir = Vector3.zero;
	Vector3 accSlipDir = Vector3.zero;

	Vector3 moveDir = Vector3.zero;

	Quaternion to;
	Camera mainCam;

	HashSet<Transform> already = new HashSet<Transform>();

	Transform[] targets;
	Transform[] prevTargets;


	Transform target;
	bool isLocked = false;

	private void Awake()
	{
		ctrl = GetComponent<CharacterController>();
		mainCam = Camera.main;
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if(hit.point.y <= transform.position.y)
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
		if (slip)
		{
			accSlipDir += slipDir * Time.deltaTime;
		}
		else
		{
			if(accSlipDir.sqrMagnitude > 0)
			{
				accSlipDir = Vector3.zero;
			}
		}
		if (!ctrl.isGrounded)
		{
			gravityAccel += 9.8f * Time.deltaTime;
		}
		else
		{
			if (gravityAccel > 0)
			{
				gravityAccel = 0;
			}
		}
		
		if(isLocked && target == null)
		{
			ResetTargets();
		}

		if (!isLocked)
		{
			Vector3 rot = mainCam.transform.eulerAngles;
			rot.x = 0;
			Vector3 v = Quaternion.Euler(rot) * moveDir;
			if(moveDir.sqrMagnitude != 0)
			{
				to = Quaternion.LookRotation(v, Vector3.up);
			}
			transform.rotation = Quaternion.RotateTowards(transform.rotation, to, spinSpd);
			ctrl.Move((accSlipDir + (v * speed) - (Vector3.up * gravityAccel * speed)) * Time.deltaTime);
		}
		else
		{
			Vector3 v = target.position - transform.position;
			v.y = 0;
			to = Quaternion.LookRotation(v, Vector3.up);
			if (to != Quaternion.identity)
			{
				transform.rotation = Quaternion.RotateTowards(transform.rotation, to, spinSpd);
			}
			ctrl.Move((accSlipDir + (transform.rotation * moveDir * speed) - (Vector3.up * gravityAccel * speed)) * Time.deltaTime);
		}

		
	}

	public void Move(InputAction.CallbackContext context)
	{
		Vector2 inp = context.ReadValue<Vector2>();
		moveDir = new Vector3(inp.x, moveDir.y, inp.y);

		if(target != null && (target.position - transform.position).sqrMagnitude >= lockOnDist * lockOnDist)
		{
			ResetTargets();
		}
	}

	public void Jump(InputAction.CallbackContext context)
	{
		if (ctrl.isGrounded && !slip)
		{
			gravityAccel = -jumpPwer;
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

						GameManager.instance.pCam.m_BindingMode = Cinemachine.CinemachineTransposer.BindingMode.LockToTargetWithWorldUp;
						GameManager.instance.pCam.m_XAxis.m_Wrap = false;
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

	void ResetTargets()
	{
		target = null;
		already.Clear();

		GameManager.instance.pCam.m_BindingMode = Cinemachine.CinemachineTransposer.BindingMode.WorldSpace;
		GameManager.instance.pCam.m_XAxis.m_Wrap = true;

		isLocked = false;
	}
}
