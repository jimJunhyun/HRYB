using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimActions : MonoBehaviour
{
	Actor self;

	GameObject holdingBow;
	GameObject equipingBow;
	public SkinnedMeshRenderer eBowRend;

	Animator animator;

	readonly int aimHash = Animator.StringToHash("Aim");
	readonly int fireHash = Animator.StringToHash("Fire");

	private void Awake()
	{
		self = GetComponentInParent<Actor>();

		holdingBow  = GameObject.Find("HoldingBow");
		equipingBow = GameObject.Find("EquipingBow");

		eBowRend = equipingBow.GetComponentInChildren<SkinnedMeshRenderer>();

		animator = holdingBow.GetComponent<Animator>();
		BowUnequip();
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
		self.move.moveDir = Vector3.zero;
		self.move.forceDir = Vector3.zero;
	}

	public void EnableInput()
	{
		GameManager.instance.pinp.ActivateInput();
	}

	public void BowEquip()
	{
		holdingBow.SetActive(true);
		equipingBow.SetActive(false);
	}

	public void BowUnequip()
	{
		holdingBow.SetActive(false);
		equipingBow.SetActive(true);
	}

	public void SetBowAimState()
	{
		animator.SetBool(aimHash, true);
	}

	public void ResetBowAimState()
	{
		animator.SetBool(aimHash, false);
	}

	public void SetFireTrigger()
	{
		animator.SetTrigger(fireHash);
	}

	public void ResetRoll()
	{
		EnableInput();
		self.life.isImmune = false;
		self.move.moveDir = Vector3.zero;
		(self.move as PlayerMove).ctrl.height *= 4f;
		(self.move as PlayerMove).ctrl.radius *= 2f;
		if (self.move.moveStat != MoveStates.Sit)
			(self.move as PlayerMove).ctrl.center *= 2f;
	}
}
