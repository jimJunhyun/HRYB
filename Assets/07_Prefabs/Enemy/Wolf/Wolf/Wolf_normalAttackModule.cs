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


		if (_nowCols != null)
		{
			_nowCols.End();
			_nowCols = null;
		}


		GameObject obj = PoolManager.GetObject($"Wolf_noraml_Attack", transform);
		if (obj.TryGetComponent(out ColliderCast cols))
		{
			_nowCols = cols;
		}

		_nowCols.Now(transform,(_life) =>
		{
			_life.DamageYY(new YinYang(0,10), DamageType.DirectHit);
		}, default, default, default, 0.3f);

		if(a== 2)
		{
			GameManager.instance.audioPlayer.PlayPoint("WolfRightAttack", transform.position);
		}
		else
		{
			
			GameManager.instance.audioPlayer.PlayPoint("WolfLeftAttack", transform.position);
			
		}

		EffectObject eff =  PoolManager.GetEffect($"Wolf_noraml_Attack{a}", transform);
		eff.Begin();
	}

	public override void OnAnimationEnd()
	{
		if(_nowCols != null)
		{

			_nowCols.End();
			_nowCols = null;
		}
	}

	public override void OnAnimationSound()
	{

	}

	public override void OnAnimationStop()
	{
		self.AI.StartExamine();
	}

	public override void Attack()
	{
		left = !left;
		int a = left ? 1 : 2;

		if (_nowCols != null)
		{
			_nowCols.End();
			_nowCols = null;
		}

	
		
		//GetActor().anim.SetAttackTrigger();
		
		GetActor().anim.Animators.SetTrigger(Animator.StringToHash($"normallAtt{a}"));

	}
	

}
