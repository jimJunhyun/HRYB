using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveStates
{
	Walk,
	Run,
	Sit,

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

	public virtual Vector3 MoveVelocity
	{
		get => moveDir * speed;
	}

	public float runSpeed;
	public float walkSpeed;
	public float crouchSpeed;

	public float speedMod = 1.0f;

	private MoveStates curStat;
	public MoveStates moveStat
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
}
