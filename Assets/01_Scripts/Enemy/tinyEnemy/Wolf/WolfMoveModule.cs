using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WolfMoveModule : MoveModule
{
	Transform _target;
	[SerializeField] private float Speed = 15f;


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
		ForceCalc();
		GravityCalc();
		transform.Translate(forceDir * Time.deltaTime, Space.World);

		
		
		if (( forceDir.x > 0 && forceDir.y > 0 && forceDir.z > 0 ) || isGrounded == false)
		{
			agent.enabled =false;
		}
		else
		{
			agent.enabled =true;
		}
		
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
