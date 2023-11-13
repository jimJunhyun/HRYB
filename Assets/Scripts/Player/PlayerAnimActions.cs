using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimActions : MonoBehaviour
{
	Actor self;

	private void Awake()
	{
		self = GetComponentInParent<Actor>();
	}



	public void LoadArrow()
	{
		(self.atk as PlayerAttack).SetBowStat();
	}
	public void FireArrow()
	{
		(self.atk as PlayerAttack).Attack();
	}

	public void DisableInput()
	{
		GameManager.instance.pinp.DeactivateInput();
	}

	public void EnableInput()
	{
		GameManager.instance.pinp.ActivateInput();
	}
}
