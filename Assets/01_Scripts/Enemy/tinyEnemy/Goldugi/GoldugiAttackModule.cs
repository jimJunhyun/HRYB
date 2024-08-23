using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldugiAttackModule : EnemyAttackModule
{
	[Header("AttackValue")]
	public float _normalATKValue = 2f;

	public override void OnAnimationEvent()
	{
		GameObject objs = PoolManager.GetObject($"Wolf_noraml_Attack", transform);
		if (objs.TryGetComponent<ColliderCast>(out _nowCols))
		{
			_nowCols.Now(transform, (_life) =>
			{
				_life.DamageYY(new YinYang(0, whiteDamage * _normalATKValue), DamageType.DirectHit);
			}, default, default, default, 0.3f);
		}

		// 이팩트
		//EffectObject eff = PoolManager.GetEffect($"", transform);
		//eff.Begin();
	}

	public override void OnAnimationEnd(AnimationEvent evt)
	{
		self.AI.StartExamine();
	}

	public override void Attack()
	{
		GetActor().anim.Animators.SetTrigger(Animator.StringToHash($"{AttackStd}"));

	}
}
