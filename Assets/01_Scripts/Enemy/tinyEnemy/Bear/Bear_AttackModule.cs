using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bear_AttackModule : EnemyAttackModule
{
	private bool left = false;

	
	public override void OnAnimationEnd()
	{
		if (_nowCols != null)
		{
			_nowCols.End();
			_nowCols = null;
		}
	}

	public override void OnAnimationEvent()
	{
		

		
		switch(AttackStd)
		{
			case "Normal":
				{
					
					int at = left ? -1 : 1;
					_nowCols.Now(transform, (_life) =>
					{
						_life.DamageYY(new YinYang(50, 0), DamageType.DirectHit);
						// 기절 ++
						Vector3 vec = _life.transform.position - transform.position;
						vec.y = 0;
						vec.Normalize();
						vec *= 4;
						vec += transform.right * at * 14;
					

						_life.GetActor().move.forceDir = vec + new Vector3(0, 20, 0);
						//_life.GetActor().move.forceDir.y = 40;

						Debug.LogError("시발시발시발시발" + _life.GetActor().move.forceDir);
					});


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
						
						GiveBuff(_life.GetActor(), StatEffID.Stun, 0.8f);


						_life.GetActor().move.forceDir = vec + new Vector3(0, 32, 0);
						//_life.GetActor().move.forceDir.y = 40;

						Debug.LogError("시발시발시발시발" + _life.GetActor().move.forceDir);
					});
					EffectObject eff = PoolManager.GetEffect($"SandBoomb", transform);
					eff.Begin();
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
		self.ai.StartExamine();
	}

	public override void ResetAttackRange(int idx)
	{

	}

	public override void SetAttackRange(int idx)
	{

	}

	public override void Attack()
	{
		
		left = !left;
		int a = left ? 1 : 2;
		string t = AttackStd;
		Debug.LogError(AttackStd);

		if (_nowCols != null)
		{
			_nowCols.End();
			_nowCols = null;
		}
		
		switch(AttackStd)
		{
			case "Normal":
				{
					GetActor().anim.Animators.SetTrigger(Animator.StringToHash($"Attack{AttackStd}{a}"));
					
					
					GameObject obj = PoolManager.GetObject($"BearNormalCollider", transform);

					if (obj.TryGetComponent(out ColliderCast cols))
					{
						_nowCols = cols;
					}
				}
				break;

			case "EX":
				{
					GetActor().anim.Animators.SetTrigger(Animator.StringToHash($"Attack{AttackStd}"));
					
					
										
					GameObject obj = PoolManager.GetObject($"BearEXCollider", transform);

					if (obj.TryGetComponent(out ColliderCast cols))
					{
						_nowCols = cols;
					}
				}
				break;
		}


		//GetActor().anim.SetAttackTrigger();

	}

}
