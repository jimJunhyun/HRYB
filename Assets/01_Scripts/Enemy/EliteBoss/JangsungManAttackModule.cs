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

	private JangSungMoveModule _jsMoveModule;
	private ColliderCast _curCols;

	private void Awake()
	{
		_jsMoveModule = GetComponent<JangSungMoveModule>();
	}

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
		//Debug.LogWarning();
		GameObject obj = PoolManager.GetObject("Jangsung" + AttackStd, transform);
		if (obj.TryGetComponent(out ColliderCast cols))
		{
			_curCols = cols;
		}
		else
		{
			Debug.LogWarning($"{obj.name} is Not  ColliderCast!!!!!");
			self.ai.StartExamine();
			return;
		}
		
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

	public override void OnAnimationStart()
	{
		_jsMoveModule.ResetDest();
	}

	public override void OnAnimationMove()
	{
		_jsMoveModule.ResetDest();
		switch (AttackStd)
		{
			case "DownAttack":
				
				break;
			case "FallDownAttack":
				_jsMoveModule.FallDownAttack();
				break;
			case "MoveAttack":
				_jsMoveModule.NormalMoveAttack();
				break;
		}
		Debug.Log("실행ㅇㅇㅇ");
	}

	public override void OnAnimationStop()
	{
		self.ai.StartExamine();	
		Debug.LogWarning("Anim Stop!!!!");
		GetActor().anim.Animators.SetBool(AttackStd, false); 

	}

	public override void OnAnimationEvent()
	{
		if (_curCols != null)
			
		{
			switch (AttackStd)
			{
				case "DownAttack":
					_curCols.CastAct += (player) => { player.AddYY(new YinYang(50, 0)); };
					break;
				case "FallDownAttack":
					_curCols.CastAct += (player) => { player.AddYY(new YinYang(100, 0)); };
					break;
				case "MoveAttack":
					_curCols.CastAct += (player) => { player.AddYY(new YinYang(10, 0)); };
					break;
			}
		}
		_curCols.Now();
		
	}

	public override void OnAnimationEnd()
	{
		_jsMoveModule.ResetDest();
		
		if (_curCols != null)
		{
			_curCols.End();
			PoolManager.ReturnObject(_curCols.gameObject);
//			Debug.LogError("푸쉬완");
		}
	}


}