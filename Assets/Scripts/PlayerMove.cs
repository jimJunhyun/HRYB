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

	public float checkThreshold = 4f;

	float gravityAccel = 0;
	float angle = 0;
	bool slip = false;

	Vector3 slipDir = Vector3.zero;
	Vector3 accSlipDir = Vector3.zero;

	Vector3 moveDir = Vector3.zero;

	Quaternion to;
	Camera mainCam;

	Transform[] targets;
	int targetIdx = -1;
	Transform target;

	private void Awake()
	{
		ctrl = GetComponent<CharacterController>();
		mainCam = Camera.main;

		
		Debug.Log("UPD");
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

		if(target == null)
		{
			GameManager.instance.pCam.m_BindingMode = Cinemachine.CinemachineTransposer.BindingMode.WorldSpace;
			GameManager.instance.pCam.m_XAxis.m_Wrap = true;
			targetIdx = -1;
		}
		
		if (targetIdx == -1)
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
			Vector3 pos;
			if(target == null)
			{
				pos = transform.position;
			}
			else
			{
				pos = target.position;
			}

			Collider[] c = Physics.OverlapSphere(pos, 30f, ~(1 << 7 | 1 << 11));
			if (c.Length > 0)
			{
				targets = c.Select(item => item.transform).OrderBy(item => (item.position - transform.position).sqrMagnitude).ToArray();
			}

			if(targets != null)
			{
				target = targets[0];
			}
			
			if(target == null)
			{
				GameManager.instance.pCam.m_BindingMode = Cinemachine.CinemachineTransposer.BindingMode.WorldSpace;
				GameManager.instance.pCam.m_XAxis.m_Wrap = true;
			}
			else
			{
				GameManager.instance.pCam.m_BindingMode = Cinemachine.CinemachineTransposer.BindingMode.LockToTargetWithWorldUp;
				GameManager.instance.pCam.m_XAxis.m_Wrap = false;
			}
		}
		//if (context.canceled)
		//{
		//	if (targets != null && targets.Length > 0)
		//	{
		//		++targetIdx;
		//		if (targetIdx == targets.Length)
		//		{
		//			targetIdx = -1;
		//			target = null;
		//			
		//			
		//		}
		//		else
		//		{
		//			target = targets[targetIdx];
		//			
		//			
		//		}
		//	}
		//	else
		//	{
		//		target = null;
		//		targetIdx = -1;
		//		GameManager.instance.pCam.m_BindingMode = Cinemachine.CinemachineTransposer.BindingMode.WorldSpace;
		//		GameManager.instance.pCam.m_XAxis.m_Wrap = true;
		//	}
		//}
	}
}
