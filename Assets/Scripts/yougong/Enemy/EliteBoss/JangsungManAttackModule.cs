using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JangsungManAttackModule : EnemyAttackModule
{
	[SerializeField] float JumpAttackDist;
	[SerializeField] float FallDownAttackDist;
	[SerializeField] private float MoveAttackDist;

	private ColliderCast _curCols;

	public void Awake()
	{

	}

	public float JumpDist()
	{
		return JumpAttackDist;
	}


	public override void Attack()
	{
		GameObject obj = PoolManager.GetObject(AttackStd, transform);
		if (obj.TryGetComponent(out ColliderCast cols))
		{
			_curCols = cols;
		}
		else
		{
			Debug.LogWarning($"{obj.name} is Not  ColliderCast!!!!!");
			return;
		}
	}

	public override void SetAttackRange(int idx)
	{
	}

	public override void ResetAttackRange(int idx)
	{
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
