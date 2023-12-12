using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JangsungManAttackModule : EnemyAttackModule
{
	[Header("Range")]
	[SerializeField] float DownAttackDist;
	[SerializeField] float FallDownAttackDist;
	[SerializeField] private float MoveAttackDist;
	

	private ColliderCast _curCols;

	public float JumpDist()
	{
		return FallDownAttackDist;
	}

	public float MoveAttack()
	{
		return MoveAttackDist;
	}

	public float DownAttack()
	{
		return DownAttackDist;
	}

	public override void Attack()
	{

		//GameObject obj = PoolManager.GetObject("Jangsung" + AttackStd, transform);
		//if (obj.TryGetComponent(out ColliderCast cols))
		//{
		//	_curCols = cols;
		//}
		//else
		//{
		//	Debug.LogWarning($"{obj.name} is Not  ColliderCast!!!!!");
		//	self.ai.StartExamine();
		//	return;
		//}
		Debug.LogWarning(AttackStd);
		GetActor().anim.SetAttackTrigger();
		GetActor().anim.Animators.SetBool(AttackStd, true);
	}

	public override void SetAttackRange(int idx)
	{
		
	}

	public override void ResetAttackRange(int idx)
	{
		
	}

	public override void OnAnimationStop()
	{
		self.ai.StartExamine();	
		Debug.LogWarning("Anim Stop!!!!");
		GetActor().anim.Animators.SetBool(AttackStd, false); 
		//PoolManager.ReturnObject(_curCols.gameObject);
	}

	public override void OnAnimationEvent()
	{
		if (_curCols != null)
		{
			_curCols.Now();
		}
		
	}

	public override void OnAnimationEnd()
	{
		if (_curCols != null)
		{
			_curCols.End();
		}
	}


}
