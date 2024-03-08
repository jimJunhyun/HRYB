using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MooseMoveModule : WolfMoveModule
{
	Transform _target;


	private bool _isMove = false;
	UnityEngine.AI.NavMeshAgent agent;

	public NavMeshAgent Agent => agent;

	private void Awake()
	{
		agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		agent.speed = Speed;
	}
	
	public void SetTarget(Transform target)
	{
		this._target = target;
		agent.updatePosition = true;
	}
	
	public override void Move()
	{
		_isMove = true;
	}
	
	public override void FixedUpdate()
	{
		// Move();
	}

	private void Update()
	{
		if (_isMove==true && _target != null)
		{
			self.anim.SetMoveState(true);
//			Debug.LogError(_target.transform.position);
			UnityEngine.AI.NavMesh.SamplePosition(_target.transform.position, out UnityEngine.AI.NavMeshHit hit, 8, UnityEngine.AI.NavMesh.AllAreas);
			agent.SetDestination(hit.position);
		}
	}

	public void StopMove()
	{
		agent.updatePosition = false;
		agent.velocity = new Vector3(0, 0, 0);
		
		
		_isMove = false;
		SetTarget(transform);
		self.anim.SetMoveState(false);
	}
}
