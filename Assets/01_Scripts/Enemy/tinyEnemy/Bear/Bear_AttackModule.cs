using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bear_AttackModule : EnemyAttackModule
{
	public override void OnAnimationEnd()
	{
	}

	public override void OnAnimationEvent()
	{
		switch(AttackStd)
		{
			case "normal":
				{
					_nowCols.Now(transform, (_life) =>
					{
						_life.DamageYY(new YinYang(50, 0), DamageType.DirectHit);
						// 기절 ++
						Vector3 vec = _life.transform.position - transform.position;
						vec.y = 0;
						vec.Normalize();



						_life.GetActor().move.forceDir = vec * 20 + new Vector3(0, 20, 0);
						//_life.GetActor().move.forceDir.y = 40;

						Debug.LogError("시발시발시발시발" + _life.GetActor().move.forceDir);
					});


					EffectObject eff = PoolManager.GetEffect($"SandBoomb", transform);
					eff.Begin();
				}
				break;

			case "EX":
				{
					_nowCols.Now(transform, (_life) =>
					{
						_life.DamageYY(new YinYang(50, 0), DamageType.DirectHit);
						// 기절 ++
						Vector3 vec = _life.transform.position - transform.position;
						vec.y = 0;
						vec.Normalize();



						_life.GetActor().move.forceDir = vec * 20 + new Vector3(0, 20, 0);
						//_life.GetActor().move.forceDir.y = 40;

						Debug.LogError("시발시발시발시발" + _life.GetActor().move.forceDir);
					});


					EffectObject eff = PoolManager.GetEffect($"SandBoomb", transform);

				}
				break;
		}


	}

	public override void OnAnimationMove()
	{
	}

	public override void OnAnimationSound()
	{
	}

	public override void OnAnimationStart()
	{
	}

	public override void OnAnimationStop()
	{
	}

	public override void ResetAttackRange(int idx)
	{

	}

	public override void SetAttackRange(int idx)
	{

	}

	public override void Attack()
	{
		string t = AttackStd;

		GameObject obj = PoolManager.GetObject($"BearAttack{t}", transform);

		if (obj.TryGetComponent(out ColliderCast cols))
		{
			_nowCols = cols;
		}


		//GetActor().anim.SetAttackTrigger();

		GetActor().anim.Animators.SetTrigger(Animator.StringToHash($"AttackStd"));

	}

}
