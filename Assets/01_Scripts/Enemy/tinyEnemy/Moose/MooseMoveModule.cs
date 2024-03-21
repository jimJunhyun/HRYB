using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MooseMoveModule : WolfMoveModule
{
	Transform _target;
	[SerializeField] private float Speed = 15f;


	private bool _isMove = false;
	UnityEngine.AI.NavMeshAgent agent;
	private CharacterController _char;

	public NavMeshAgent Agent => agent;
	public CharacterController Character => _char;

	private void Awake()
	{
		agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		_char = GetComponent<CharacterController>();
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

		if (_isCanMove != true)
		{
			if (( forceDir.y > 0 ) || isGrounded == false)
			{
				Character.enabled = true;
				agent.enabled =false;
			
				ForceCalc();
				GravityCalc();
				//transform.Translate(forceDir * Time.deltaTime, Space.World);
				if (_isCanMove == false)
				{
					Character.Move((forceDir) * Time.deltaTime);
				}
//			Debug.LogError(forceDir);
			}
			else
			{
				Character.enabled = false;
				agent.enabled =true;
			}
		}

		
	}

	private void Update()
	{
		if (_isMove==true && _target != null && agent.enabled==true)
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
