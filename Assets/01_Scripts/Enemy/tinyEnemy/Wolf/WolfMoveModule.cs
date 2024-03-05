using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfMoveModule : MoveModule
{
	Transform _target;
	[SerializeField] private float Speed = 15f;


	private bool _isMove = false;
	UnityEngine.AI.NavMeshAgent agent;
	
	private void Awake()
	{
		agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		agent.speed = Speed;
	}
	
	public void SetTarget(Transform target)
	{
		this._target = target;
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
		_isMove = false;
		SetTarget(transform);
		self.anim.SetMoveState(false);
	}
}
