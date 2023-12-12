using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscaperMove : MoveModule
{
	Transform target;

	public override void Move()
	{
		if (target != null)
		{

			Vector3 v = (target.position - transform.position);
			v.y = 0;
			moveDir = -v.normalized;
			transform.Translate(moveDir * Speed * Time.deltaTime, Space.World); // NavMesh사용예정
			if (moveDir.sqrMagnitude > 0.01)
			{
				transform.rotation = Quaternion.LookRotation(moveDir);
			}
			GetActor().anim.SetMoveState(1);
		}

	}

	public void SetTarget(Transform t)
	{
		target = t;
	}

}
