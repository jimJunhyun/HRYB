using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf_normalAttackModule : EnemyAttackModule
{
	private bool left = false;
	
	public override void SetAttackRange(int idx)
	{

	}

	public override void ResetAttackRange(int idx)
	{

	}

	public override void OnAnimationStart()
	{

	}

	public override void OnAnimationMove()
	{

	}

	public override void OnAnimationEvent()
	{

	}

	public override void OnAnimationEnd()
	{

	}

	public override void OnAnimationSound()
	{

	}

	public override void OnAnimationStop()
	{

	}

	public override void Attack()
	{
		left = !left;
		int a = left == true ? 1 : 0;
		GameObject obj = PoolManager.GetObject($"Wolf_noraml{a}", transform);

		if (obj.TryGetComponent(out ColliderCast cols))
		{
			
		}
	}

}
