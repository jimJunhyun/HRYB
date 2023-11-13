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

	public float Speed
	{

		get => speed * speedMod;
		set => speed = value;
	}

    public Vector3 moveDir = Vector3.zero;

	public Vector3 forceDir = Vector3.zero;

	public float runSpeed;
	public float walkSpeed;
	public float crouchSpeed;

	public float speedMod = 1.0f;

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
		
	}

	public virtual void ForceCalc()
	{
		if(forceDir.sqrMagnitude > 0.001f)
		{
			
			Vector3 antiForce = -(forceDir) * 3f * Time.deltaTime;
			forceDir += antiForce;
		}
		else
		{
			forceDir = Vector3.zero;
		}
	}
}
