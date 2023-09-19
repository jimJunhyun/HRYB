using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
	CharacterController ctrl;
	public float speed = 4f;
	public float spinSpd = 300f;
	public float jumpPwer = 20f;

	public float slipThreshold = 45f;
	public float slipPower = 4f;

	float gravityAccel = 0;
	float angle = 0;
	bool slip = false;

	Vector3 slipDir = Vector3.zero;
	Vector3 accSlipDir = Vector3.zero;

	Vector3 moveDir = Vector3.zero;
	Vector3 turnDir = Vector3.zero;

	private void Awake()
	{
		ctrl = GetComponent<CharacterController>();
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
		moveDir.y = -gravityAccel;
		ctrl.Move((accSlipDir + (transform.rotation * moveDir * speed)) * Time.deltaTime);
		transform.Rotate(turnDir * spinSpd * Time.deltaTime);
	}

	public void Move(InputAction.CallbackContext context)
	{
		Vector2 inp = context.ReadValue<Vector2>();
		moveDir = new Vector3(inp.x, moveDir.y, inp.y);
	}

	public void Turn(InputAction.CallbackContext context)
	{
		Vector2 inp = context.ReadValue<Vector2>();
		turnDir = new Vector3(0, inp.x, 0);
	}

	public void Jump(InputAction.CallbackContext context)
	{
		if (ctrl.isGrounded)
		{
			gravityAccel = -jumpPwer;
		}
	}
}
