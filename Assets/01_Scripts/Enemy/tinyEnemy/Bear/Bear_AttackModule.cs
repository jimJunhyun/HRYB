using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bear_AttackModule : EnemyAttackModule
{
	private bool left = false;
	
	[SerializeField] GameObject _firePos;

	private int tempCount = 0;
	
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
					
					if(_nowCols != null)
					{
						_nowCols.End();
						_nowCols = null;
					}

					int at = left ? -1 : 1;

					GameObject objs = PoolManager.GetObject("BearNormalCollider", transform);

					if(at == -1)
					{
						GameManager.instance.audioPlayer.PlayPoint("BearAttackLeft", transform.position);
					}
					else
					{
						GameManager.instance.audioPlayer.PlayPoint("BearAttackRight", transform.position);
					}
					if(objs.TryGetComponent<ColliderCast>(out _nowCols))
					{
						_nowCols.Now(transform, (_life) =>
						{
							_life.DamageYY(new YinYang(0, 15), DamageType.DirectHit);
							// 기절 ++
							Vector3 vec = _life.transform.position - transform.position;
							vec.y = 0;
							vec.Normalize();
							vec *= 4;
							vec += transform.right * at * 14;

							_life.GetActor().move.forceDir = vec + new Vector3(0, 3, 0);
							//_life.GetActor().move.forceDir.y = 40;

							//Debug.LogError("시발시발시발시발" + _life.GetActor().move.forceDir);
						}, default, default, 1f);
					}

				
				}
				break;

			case "EX":
				{

					if (tempCount == 0)
					{
						EffectObject eff = PoolManager.GetEffect($"FlameStream", _firePos.transform);
						eff.Begin();
						tempCount++;
					}
					else
					{
						GameObject obj = PoolManager.GetObject($"BearFireCol", _firePos.transform); ;

						if (obj.TryGetComponent(out ColliderCast cols))
						{
							_nowCols = cols;
						}


						obj.transform.position = _firePos.transform.position;
						obj.transform.LookAt(self.AI.player.transform);
						obj.transform.parent = null;

						_nowCols.Now(transform, (_life) =>
						{
							_life.DamageYY(new YinYang(0, 2), DamageType.DirectHit);
							// 기절 ++
							Vector3 vec = _life.transform.position - transform.position;
							vec.y = 0;
							vec.Normalize();


							_life.GetActor().move.forceDir = vec * 2; //+ new Vector3(0, 32, 0);
																	  //_life.GetActor().move.forceDir.y = 40;
						}, default, default, 0.2f) ;

					}
					
					
				}
				break;
			case "EX2":
				{
					
					GameObject obj = PoolManager.GetObject($"BearEXCollider", transform); ;

					GameManager.instance.audioPlayer.PlayPoint("BearAttackRight", transform.position);

					if (obj.TryGetComponent(out ColliderCast cols))
					{
						_nowCols = cols;
					}
					
					_nowCols.Now(transform, (_life) =>
					{
						_life.DamageYY(new YinYang(0, 30), DamageType.DirectHit);
						// 기절 ++
						Vector3 vec = _life.transform.position - transform.position;
						vec.y = 0;
						vec.Normalize();
						
						//GiveBuff(_life.GetActor(), StatEffID.Stun, 0.8f);


						_life.GetActor().move.forceDir = vec + new Vector3(0, 7, 0);
						//_life.GetActor().move.forceDir.y = 40;
					}, default, default, 1f);
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
		self.AI.StartExamine();
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
		//Debug.LogError(AttackStd);
		tempCount = 0;
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
				}
				break;

			case "EX":
				{
					//GetActor().anim.Animators.SetTrigger(Animator.StringToHash($"Attack{AttackStd}"));
					GetActor().anim.Animators.SetTrigger(Animator.StringToHash($"Buff"));
					
				}
				break;
			case "EX2":
				{
					//GetActor().anim.Animators.SetTrigger(Animator.StringToHash($"Attack{AttackStd}"));
					GetActor().anim.Animators.SetTrigger(Animator.StringToHash($"AttackEX"));
					
				}
				break;
		}


		//GetActor().anim.SetAttackTrigger();

	}

}
