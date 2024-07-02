using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EscaperMove : EnemyMoveModule
{
	Coroutine _escapeCO;

	public override void SetTarget(Transform target, MoveStates moves = MoveStates.Run)
	{
		_target = target;
		if (Agent.enabled)
		{
			moveStat = moves;
			Agent.isStopped = false;
			Agent.updatePosition = true;
			Agent.updateRotation = false;
		}
	}

	IEnumerator EscapeSetter()
	{
		Vector3 v = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
		v.y = 0;
		moveDir = v.normalized * 2;


		yield return new WaitForSeconds(Random.Range(0.4f,1.2f));
		_escapeCO = null;
	}

	public override void Move()
	{
		_isMove = true;


		//Debug.LogError($"모브브느아므리ㅡ {_isMove == true} {_target != null}  {Agent.enabled == true}");
		if (_isMove == true && _target != null && Agent.enabled == true)
		{
			if (_escapeCO == null)
				_escapeCO = StartCoroutine(EscapeSetter());


			if (moveDir.sqrMagnitude > 0.01)
			{
				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDir),Time.deltaTime * 9);
			}
			self.anim.SetMoveState(true);
			UnityEngine.AI.NavMesh.SamplePosition(transform.position + moveDir, out UnityEngine.AI.NavMeshHit hit, 1f, UnityEngine.AI.NavMesh.AllAreas);
			Agent.SetDestination(hit.position);
		}
		else
		{
			self.anim.SetMoveState(false);
			StopMove();
		}
	}

}
