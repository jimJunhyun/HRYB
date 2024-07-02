using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EscaperMove : EnemyMoveModule
{
	Transform _target;


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


	public override void SetTarget(Transform target, MoveStates moves = MoveStates.Run)
	{
		_target = target;
		if (agent.enabled)
		{
			moveStat = moves;
			agent.isStopped = false;
			agent.updatePosition = true;
			agent.updateRotation = false;
		}
	}

	public override void Move()
	{
		_isMove = true;


		Debug.LogError($"모브브느아므리ㅡ {_isMove == true} {_target != null}  {Agent.enabled == true}");
		if (_isMove == true && _target != null && Agent.enabled == true)
		{
			Vector3 v = (_target.position - transform.position);
			v.y = 0;
			moveDir = -v.normalized;

			if (moveDir.sqrMagnitude > 0.01)
			{
				transform.rotation = Quaternion.LookRotation(moveDir);
			}

			self.anim.SetMoveState(true);
			UnityEngine.AI.NavMesh.SamplePosition(moveDir*3, out UnityEngine.AI.NavMeshHit hit, 1f, UnityEngine.AI.NavMesh.AllAreas);
			Agent.SetDestination(hit.position);
		}
		else
		{
			self.anim.SetMoveState(false);
			StopMove();
		}
	}

}
