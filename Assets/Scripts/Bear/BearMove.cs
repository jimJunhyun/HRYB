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
			
			moveDir = (target.position - transform.position).normalized;
			transform.Translate(moveDir * Speed * Time.deltaTime, Space.World);
			if(moveDir.sqrMagnitude > 0.01)
			{
				transform.rotation = Quaternion.LookRotation(moveDir);
			}
		}
		
	}

	public void SetTarget(Transform t)
	{
		target = t;
	}
}
