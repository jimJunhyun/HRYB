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
		int a = left ? 2 : 1;
		
		_nowCols.Now(transform,(_life) =>
		{
			_life.DamageYY(new YinYang(20,0), DamageType.DirectHit);
		});
		EffectObject eff =  PoolManager.GetEffect($"Wolf_noraml_Attack{a}", transform.GetChild(0));
		eff.Begin();
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
		int a = left ? 1 : 2;
		
		GameObject obj = PoolManager.GetObject($"Wolf_noraml_Attack", transform);
		
		if (obj.TryGetComponent(out ColliderCast cols))
		{
			_nowCols = cols;
		}
		
		
		//GetActor().anim.SetAttackTrigger();
		
		GetActor().anim.Animators.SetTrigger(Animator.StringToHash($"normallAtt{a}"));

	}
	

}
