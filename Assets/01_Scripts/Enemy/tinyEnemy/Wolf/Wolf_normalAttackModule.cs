using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf_normalAttackModule : EnemyAttackModule
{
	private bool left = false;
	private ColliderCast _nowCols;
	
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
		left = !left;
		int a = left == true ? 1 : 2;
		_nowCols.Now((_life) =>
		{
			_life.DamageYY(20, 0, DamageType.DirectHit);
		});
		EffectManager.GetObject($"Wolf_noraml_Attack{a}", transform);
	}

	public override void OnAnimationEnd()
	{
		_nowCols.End();
	}

	public override void OnAnimationSound()
	{

	}

	public override void OnAnimationStop()
	{
		self.ai.StartExamine();	
	}

	public override void Attack()
	{
		left = !left;
		int a = left == true ? 1 : 2;
		
		GameObject obj = PoolManager.GetObject($"Wolf_noraml_Attack", transform);
		
		if (obj.TryGetComponent(out ColliderCast cols))
		{
			_nowCols = cols;
		}
		
		
		GetActor().anim.SetAttackTrigger();
		if(left)
			GetActor().anim.Animators.SetTrigger(Animator.StringToHash(AttackStd + $"{a}"));
		else
			GetActor().anim.Animators.SetTrigger(Animator.StringToHash(AttackStd + $"{a}"));
	}
	

}
