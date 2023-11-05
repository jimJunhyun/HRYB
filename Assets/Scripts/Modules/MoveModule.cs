using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveStates
{
	Idle,
	Walk,
	Run,
	Sit,

}

public class MoveModule : Module
{
    protected float speed = 4f;
    public Vector3 moveDir = Vector3.zero;

	public float runSpeed;
	public float walkSpeed;
	public float crouchSpeed;

	private MoveStates curStat;
	public MoveStates moveStat
	{
		get => curStat;
		protected set
		{
			switch (value)
			{
				case MoveStates.Idle:
					curStat = MoveStates.Idle;
					break;
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

	public virtual void Move()
	{
		
	}
}
