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
		_missile.Clear();

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

					GetActor().anim.Animators.SetBool(AttackStd, false);
					self.ai.StartExamine();
				}
				break;
			case "Root":
				{

					GetActor().anim.Animators.SetBool(AttackStd, false);
					self.ai.StartExamine();
				}
				break;
			case "Barrier":
				{
					GetActor().anim.Animators.SetBool(AttackStd, false);
					self.ai.StartExamine();
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
			case "Root":
				{
					StartCoroutine(RootPatton());
				}
				break;
		}



	}


	IEnumerator RootPatton()
	{
		JangsungGirlLifeModule lf = GetComponent<JangsungGirlLifeModule>();
		lf.BarrierON(5);

		// 바닥 범위 보여주기 <= 에니메이션 처리


		yield return new WaitForSeconds(3f);

		for(int i =0; i < 50; i++)
		{

			if(lf.IsBarrier == false)
			{
				break;
			}


			float x;
			float y;
			bool escape;
			yield return new WaitForSeconds(0.4f);
			do
			{
				x = transform.position.x + Random.Range(-25.0f, 25.0f);
				y = transform.position.z + Random.Range(-25.0f, 25.0f);
				escape = Mathf.Pow(25, 2) > (Mathf.Pow(transform.position.x - x, 2) + Mathf.Pow(transform.position.z - y, 2));

			} while (escape == false);
			// 생성
			StartCoroutine(SummonPuri(x, y));
		}

		OnAnimationEnd();
	}

	IEnumerator SummonPuri(float x, float y)
	{
		GameObject obj = PoolManager.GetObject("MiddleBoxDecal", transform);
		if (obj.TryGetComponent<BoxDecal>(out BoxDecal box))
		{
			box.SetUpDecal(new Vector3(x,0.15f,y), Quaternion.identity, new Vector3(0.5f,0.5f,0.5f), new Vector3(0,0,0), new Vector3(1,1,1));
			box.StartDecal(0.6f);
		}

		yield return new WaitForSeconds(0.6f);
		GameObject objs = PoolManager.GetObject("JangsungPuri", new Vector3(x, transform.position.y, y), Quaternion.identity);
		objs.GetComponent<ColliderCast>().Now((player) =>
		{
			player.AddYYBase(new YinYang(30, 0));
		}, -1, 2f, 3f);
	}

	public override void OnAnimationMove()
	{
	}

	public override void OnAnimationSound()
	{
	}

	public override void OnAnimationStart()
	{
		GetActor().anim.SetAttackTrigger();
		GetActor().anim.Animators.SetBool(AttackStd, true);
	}

	public override void OnAnimationStop()
	{
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
