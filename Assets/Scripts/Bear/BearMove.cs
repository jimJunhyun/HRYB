using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearMove : MoveModule
{
	Transform target;
	
	public override void Move()
	{
		if(target != null)
		{
			
			Vector3 v = (target.position - transform.position);
			v.y = 0;
			moveDir = v.normalized;
			transform.Translate(moveDir * Speed * Time.deltaTime, Space.World); // NavMesh사용예정
			if(moveDir.sqrMagnitude > 0.01)
			{
				transform.rotation = Quaternion.LookRotation(moveDir);
			}
			GetActor().anim.SetMoveState(1);
		}
		
	}

	public void LookAt(Transform t)
	{
		Vector3 lookPos = t.position - transform.position;
		lookPos.y = transform.position.y;
		transform.rotation = Quaternion.LookRotation(lookPos);
	}

	public void SetTarget(Transform t)
	{
		target = t;
	}
}
