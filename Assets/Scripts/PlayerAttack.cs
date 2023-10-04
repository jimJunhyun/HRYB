using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public void OnAim(InputAction.CallbackContext context)
	{
		if (context.started)
		{

		}
		if (context.canceled)
		{

		}
	}

	public void OnAttack(InputAction.CallbackContext context)
	{
		if (context.started)
		{

		}
		if (context.performed)
		{

		}
		if (context.canceled)
		{

		}
	}
}
