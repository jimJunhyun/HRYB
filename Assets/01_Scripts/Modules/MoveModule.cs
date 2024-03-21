using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveStates
{
	Walk,
	Run,
	Sit,
	Climb,

}

public class MoveModule : Module
{
    protected float speed = 4f;

	const string GRASSLAYERNAME = "Grass";
	const string STONELAYERNAME = "Stone";

	public virtual float Speed
	{

		get => speed * moveModuleStat.Speed;
		set => speed = value;
	}

    public Vector3 moveDir = Vector3.zero;

	public Vector3 forceDir = Vector3.zero;

	public float runSpeed;
	public float walkSpeed;
	public float crouchSpeed;

	public bool gravity = false;
	public float groundThreshold = 0.5f;

	public virtual bool isGrounded
	{
		get=>Physics.Raycast(transform.position + new Vector3(0, 0.1f,0), Vector3.down, groundThreshold, 1<< GameManager.GROUNDLAYER);
	}
	public ModuleController moveModuleStat = new ModuleController(false);

	protected MoveStates curStat;
	public virtual MoveStates moveStat
	{
		get => curStat;
		protected set
		{
			switch (value)
			{
				case MoveStates.Walk:
					curStat = MoveStates.Walk;
					speed = walkSpeed;
					break;
				case MoveStates.Run:
					curStat = MoveStates.Run;
					speed = runSpeed;
					break;
				case MoveStates.Sit:
					curStat = MoveStates.Sit;
					speed = crouchSpeed;
					break;
				default:
					break;
			}
		}
	}

	public bool idling
	{
		get => moveDir.sqrMagnitude < 0.1f;
	}


	public virtual void Move()
	{
		ForceCalc();
		GravityCalc();
		transform.Translate(forceDir * Time.fixedDeltaTime, Space.World);
	}

	public virtual void FixedUpdate()
	{
		Move();
	}

	public virtual void GravityCalc()
	{
		if (gravity && !isGrounded)
		{
			forceDir.y -= GameManager.GRAVITY * Time.deltaTime;
		}
		else if (isGrounded && forceDir.y <0)
		{
			forceDir.y = 0f;
		}
	}

	public virtual void ForceCalc()
	{
		if(forceDir.sqrMagnitude > 0.01f)
		{
			Vector3 v = forceDir;
			v.y = 0;
			if(v.sqrMagnitude > 0.01f)
			{
				Vector3 antiForce = -(v) * GameManager.instance.forceResistance * Time.deltaTime;
				forceDir += antiForce;
			}
		}
		else
		{
			forceDir = Vector3.zero;
		}
	}
	
	public void LookAt(Transform t)
	{
		Vector3 lookPos = t.position - transform.position;
		lookPos.y = transform.position.y;
		transform.rotation = Quaternion.LookRotation(lookPos);
	}

	public override void ResetStatus()
	{
		base.ResetStatus();
		moveStat  =MoveStates.Walk;
		moveDir = Vector3.zero;
		forceDir = Vector3.zero;
		moveModuleStat.CompleteReset();
	}
}
