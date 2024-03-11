using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Moose_normalAttackModule : EnemyAttackModule
{
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
		_nowCols.Now(transform,(_life) =>
		{
			_life.DamageYY(new YinYang(50,0), DamageType.DirectHit);
			// 기절 ++
			Vector3 vec = _life.transform.position - transform.position;
			vec.Normalize();
			vec *= 20;
			vec.y += 40;
			
			
			_life.GetActor().move.forceDir += vec;
			//_life.GetActor().move.forceDir.y = 40;
			
			Debug.LogError(_life.GetActor().move.forceDir);
		});
		EffectObject eff =  EffectManager.Instance.GetObject($"SandBoomb", transform);
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
		GameObject obj = PoolManager.GetObject($"Moose_noraml_Attack", transform);
		
		if (obj.TryGetComponent(out ColliderCast cols))
		{
			_nowCols = cols;
		}
		
		
		//GetActor().anim.SetAttackTrigger();
		
		GetActor().anim.Animators.SetTrigger(Animator.StringToHash($"Attack1"));

	}
}
