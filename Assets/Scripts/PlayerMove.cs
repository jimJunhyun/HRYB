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

	float gravityAccel = 0;

	Vector3 moveDir = Vector3.zero;
	Vector3 turnDir = Vector3.zero;

	private void Awake()
	{
		ctrl = GetComponent<CharacterController>();
	}

	private void Update()
	{
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
		ctrl.Move(transform.rotation * moveDir * speed * Time.deltaTime);
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
