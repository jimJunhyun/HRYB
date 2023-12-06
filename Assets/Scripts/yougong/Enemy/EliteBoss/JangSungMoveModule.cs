using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JangSungMoveModule : MoveModule
{
	Transform target;
	UnityEngine.AI.NavMeshAgent agent;

	[Header("fIND")] [SerializeField] private float _findPlayerRange = 30;
	
	[Header("Stat")] 
	[SerializeField] private float _normalSpeed = 3.5f;
	[SerializeField] private float _fallDownMoveSpeed = 9f;
	public override float Speed { get => base.Speed; set{ base.Speed = value; agent.speed = base.Speed; } }

	private void Awake()
	{
		agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		agent.speed = Speed;
	}

	public override void Move()
	{

		
	}

	public void PowerUp()
	{
		_normalSpeed *= 2;
		_fallDownMoveSpeed *= 2;
	}

	public float FindPlayer()
	{
		return _findPlayerRange;
	}

	public void FallDownAttack()
	{
		if(target != null)
		{
			Vector3 vec = target.position;
			UnityEngine.AI.NavMesh.SamplePosition( vec, out UnityEngine.AI.NavMeshHit hit, 5f, UnityEngine.AI.NavMesh.AllAreas);
			agent.speed = _fallDownMoveSpeed;
			agent.SetDestination(hit.position);
			//GetActor().anim.SetMoveState();
		}
	}

	public void NormalMoveAttack()
	{
		if(target != null)
		{
			UnityEngine.AI.NavMeshHit hit;
			
			Vector3 vec = (transform.position - target.position);
			vec.y = 0;
			
			
			vec = vec.normalized * 15 + transform.position;

			if (Vector3.Distance(vec, target.position) < 5)
			{
				
				UnityEngine.AI.NavMesh.SamplePosition(target.position, out  hit, 5f, UnityEngine.AI.NavMesh.AllAreas);
			}
			else
			{
				UnityEngine.AI.NavMesh.SamplePosition(vec, out hit, 5f, UnityEngine.AI.NavMesh.AllAreas);
				
			}
			
			agent.speed = _normalSpeed;
			agent.SetDestination(hit.position);
			//GetActor().anim.SetMoveState();
		}
	}

	public void ResetDest()
	{
		UnityEngine.AI.NavMesh.SamplePosition(transform.position, out UnityEngine.AI.NavMeshHit hit, 5f, UnityEngine.AI.NavMesh.AllAreas);
		agent.SetDestination(hit.position);
	}
}
