using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JangsungGirlAttack : EnemyAttackModule
{

	[SerializeField] List<Transform> mumukPos = new();

	List<JangsungMumukMissile> _missile = new();
	
	int fireIndex = 0;


	public override void Attack()
	{
		fireIndex = 0;
		_missile = new();

	}

	public override void OnAnimationEnd()
	{
		switch (AttackStd)
		{
			case "mumuk":
				{
					for (int i = 0; i < _missile.Count; ++i)
					{
						_missile[i].Fire();
					}

					self.ai.StartExamine();
					GetActor().anim.Animators.SetBool(AttackStd, false);
				}
				break;
		}
	}

	public override void OnAnimationEvent()
	{

		switch (AttackStd)
		{
			case "mumuk":
				{
					if(fireIndex == 0)
					{
						GameObject obj = PoolManager.GetObject("Jangsung" + AttackStd, transform);
						_missile.Add(obj.GetComponent<JangsungMumukMissile>());
						_missile[fireIndex].Init(mumukPos[fireIndex], self.ai.player.transform, 5);
						fireIndex++;
					}
					else
					{
						GameObject obj1 = PoolManager.GetObject("Jangsung" + AttackStd, transform);
						_missile.Add(obj1.GetComponent<JangsungMumukMissile>());
						_missile[fireIndex].Init(mumukPos[fireIndex], self.ai.player.transform, 5);
						fireIndex++;

						GameObject obj2 = PoolManager.GetObject("Jangsung" + AttackStd, transform);
						_missile.Add(obj2.GetComponent<JangsungMumukMissile>());
						_missile[fireIndex].Init(mumukPos[fireIndex], self.ai.player.transform, 5);
						fireIndex++;
					}

				}
				break;
		}



	}

	public override void OnAnimationMove()
	{
		throw new System.NotImplementedException();
	}

	public override void OnAnimationSound()
	{
		throw new System.NotImplementedException();
	}

	public override void OnAnimationStart()
	{
		GetActor().anim.SetAttackTrigger();
		GetActor().anim.Animators.SetBool(AttackStd, true);
	}

	public override void OnAnimationStop()
	{
		throw new System.NotImplementedException();
	}

	public override void ResetAttackRange(int idx)
	{
		throw new System.NotImplementedException();
	}

	public override void SetAttackRange(int idx)
	{
		throw new System.NotImplementedException();
	}
}
