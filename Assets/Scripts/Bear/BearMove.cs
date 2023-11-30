using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BearMove : MoveModule
{
	Transform target;
	NavMeshAgent agent;

	public override float Speed { get => base.Speed; set{ base.Speed = value; agent.speed = base.Speed; } }

	private void Awake()
	{
		 agent = GetComponent<NavMeshAgent>();
		agent.speed = Speed;
	}

	public override void Move()
	{
		if(target != null)
		{

			NavMesh.SamplePosition(target.position, out NavMeshHit hit, 5f, NavMesh.AllAreas);
			agent.SetDestination(hit.position);
			GetActor().anim.SetMoveState(1);
		}
		
	}

	public void ResetDest()
	{
		NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas);
		agent.SetDestination(hit.position);
	}



	public void SetTarget(Transform t)
	{
		target = t;
		if(t == null)
		{
			ResetDest();
		}
	}
}
