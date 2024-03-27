using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMoveModule : MoveModule
{
	Transform _target;
	[SerializeField] private float Speed = 15f;


	private bool _isMove = false;
	UnityEngine.AI.NavMeshAgent _agent;

	NavMeshAgent agent
	{
		get
		{
			if (_agent == null)
			{
				_agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
			}

			return _agent;
		}
	}
	private CharacterController _char;

	public UnityEngine.AI.NavMeshAgent Agent => agent;
	public CharacterController Character => _char;

	private void Awake()
	{

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
		ForceCalc();
		GravityCalc();
		Debug.LogError("FORCEdIR	" + forceDir + "d : " + isGrounded);
		Character.Move((forceDir) * Time.deltaTime);
		if (_isCanMove != true)
		{
			if ((forceDir.y > 0) || isGrounded == false)
			{
				Character.enabled = true;
				agent.enabled = false;

				
				//transform.Translate(forceDir * Time.deltaTime, Space.World);
				if (_isCanMove == false)
				{
					
				}
				//			Debug.LogError(forceDir);
			}
			else
			{
				Character.enabled = false;
				agent.enabled = true;
			}
		}


	}

	private void Update()
	{
		if (_isMove == true && _target != null && agent.enabled == true)
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
