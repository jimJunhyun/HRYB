using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangGwiAttackModule : EnemyAttackModule
{
	[Header("AttackValue")]
	public float _normalATKValue = 1.4f;
	public float _dashATKValue = 3.2f;

	int t = 0;

	public override void OnAnimationEnd(AnimationEvent evt)
	{
		self.AI.StartExamine();
	}

	public override void OnAnimationEvent(AnimationEvent evt)
	{
		switch (evt.stringParameter)
		{
			case "1":
				{
					if (t % 2 == 0)
					{
						EffectObject ebg = PoolManager.GetEffect("Slash11WhiteLeft", transform);
						ebg.Begin();
						ebg.transform.parent = null;
					}
					else
					{
						EffectObject ebg = PoolManager.GetEffect("Slash11WhiteRight", transform);
						ebg.Begin();
						ebg.transform.parent = null;
					}
					t++;
					GameObject objs = PoolManager.GetObject("BearNormalCollider", transform);

					if (objs.TryGetComponent<ColliderCast>(out _nowCols))
					{
						_nowCols.Now(transform, (_life) =>
						{
							_life.DamageYY(new YinYang(0, whiteDamage * _normalATKValue), DamageType.DirectHit);

						}, default, default, default, 0.4f);
					}
				}
				break;
			case "2":
				{
					GameObject objs = PoolManager.GetObject("BearNormalCollider", transform);

					if (objs.TryGetComponent<ColliderCast>(out _nowCols))
					{
						self.move.forceDir = transform.forward * 9f;// + new Vector3(0, 3, 0);
						_nowCols.Now(transform, (_life) =>
						{
							_life.DamageYY(new YinYang(0, whiteDamage * _dashATKValue), DamageType.DirectHit);

						}, default, default, default, 1f);
					}
				}
				break;
		}
	}

	public override void OnAnimationMove(AnimationEvent evt)
	{
	}

	public override void OnAnimationSound(AnimationEvent evt)
	{
	}

	public override void OnAnimationStart(AnimationEvent evt)
	{
	}

	public override void OnAnimationStop(AnimationEvent evt)
	{
		self.AI.StartExamine();
	}

	public override void Attack()
	{
		switch (AttackStd)
		{
			case "NormallAtt":
				{
					GetActor().anim.Animators.SetTrigger(Animator.StringToHash($"NormalAttack"));
				}
				break;

			case "DashAtt":
				{
					//GetActor().anim.Animators.SetTrigger(Animator.StringToHash($"Attack{AttackStd}"));
					GetActor().anim.Animators.SetTrigger(Animator.StringToHash($"DashAttack"));

				}
				break;
		}
	}
}
