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

		GetActor().anim.Animators.SetBool(AttackStd, true);
		switch (AttackStd)
		{

			case "Root":
				{
					GameManager.instance.audioPlayer.PlayPoint("JSGBarrier", transform.position);
				}
				break;
			case "Barrier":
				{
					GameManager.instance.audioPlayer.PlayPoint("JSGBarrier", transform.position);
				}
				break;
		}
	}

	public override void OnAnimationEnd(AnimationEvent evt)
	{
		StopAllCoroutines();
		switch (AttackStd)
		{
			case "mumuk":
				{
					for (int i = 0; i < _missile.Count; ++i)
					{
						_missile[i].Fire();
					}

					GetActor().anim.Animators.SetBool(AttackStd, false);
					StartCoroutine(WaeUpCo(2f));
				}
				break;
			case "Root":
				{

					GetActor().anim.Animators.SetBool(AttackStd, false);

					GetComponent<JangsungGirlAI>()._friend.GetComponent<JangsungLifeModule>().BarrierOff();
					StartCoroutine(WaeUpCo(6f));
				}
				break;
			case "Barrier":
				{
					GetActor().anim.Animators.SetBool(AttackStd, false);
					GetComponent<JangsungGirlAI>()._friend.GetComponent<JangsungLifeModule>().BarrierOff();
					StartCoroutine(WaeUpCo(4f));
				}
				break;
		}
		GetComponent<JangsungGirlLifeModule>().DeleteBarrier();
	}

	IEnumerator WaeUpCo(float t)
	{
		yield return new WaitForSeconds(t);
		self.AI.StartExamine();
	}


	public override void OnAnimationEvent(AnimationEvent evt)
	{

		switch (AttackStd)
		{
			case "mumuk":
				{
					//Debug.LogError("생성중");
					StartCoroutine(SeedPatton());

				}
				break;
			case "Root":
				{
					StartCoroutine(RootPatton(evt));
				}
				break;
		}



	}

	IEnumerator SeedPatton()
	{

		if (fireIndex == 0)
		{
			GameObject obj = PoolManager.GetObject("MumukMissile", mumukPos[fireIndex].position, mumukPos[fireIndex].rotation);
			_missile.Add(obj.GetComponent<JangsungMumukMissile>());

			//Debug.LogError(self);
			_missile[fireIndex].Init(mumukPos[fireIndex], self.AI.player.transform, 20, DamageType.DirectHit);
			fireIndex++;
		}
		else
		{
			GameObject obj1 = PoolManager.GetObject("MumukMissile", mumukPos[fireIndex].position, mumukPos[fireIndex].rotation);
			_missile.Add(obj1.GetComponent<JangsungMumukMissile>());
			_missile[fireIndex].Init(mumukPos[fireIndex], self.AI.player.transform, 15 * fireIndex, DamageType.DirectHit);
			fireIndex++;

			yield return null;
			GameObject obj2 = PoolManager.GetObject("MumukMissile", mumukPos[fireIndex].position, mumukPos[fireIndex].rotation);
			_missile.Add(obj2.GetComponent<JangsungMumukMissile>());
			_missile[fireIndex].Init(mumukPos[fireIndex], self.AI.player.transform, 15 * (fireIndex - 1), DamageType.DirectHit);
			fireIndex++;
		}

		yield return null;
	}


	IEnumerator RootPatton(AnimationEvent evt)
	{
		JangsungGirlLifeModule lf = GetComponent<JangsungGirlLifeModule>();
		lf.BarrierON(4);

		// 바닥 범위 보여주기 <= 에니메이션 처리

		GameManager.instance.loader.FadeInOut("지하여장군을 공격해서 보호막을 제거하세요!!!", 0.8f);

		GameObject objs = PoolManager.GetObject("JSRootATK", transform);
		objs.GetComponent<ColliderCast>().Now(transform, (player) =>
		{
			player.DamageYY(0, 5, DamageType.DirectHit);
			GiveBuff(player.GetActor(), StatEffID.Stun, 1.2f);
		}, null, -1, -1, 0.5f);

		yield return new WaitForSeconds(1.5f);
		yield return new WaitForSeconds(1.5f);

		for (int i = 0; i < 50; i++)
		{
			yield return new WaitForSeconds(0.5f - 0.01f * i);

			if (i < 25)
			{
				if (i % 5 == 0)
				{
					GameObject obj1 = PoolManager.GetObject("MumukMissile", mumukPos[Random.Range(0, mumukPos.Count)].position, mumukPos[Random.Range(0, mumukPos.Count)].rotation);
					JangsungMumukMissile missile = obj1.GetComponent<JangsungMumukMissile>();
					missile.Init(mumukPos[0], self.AI.player.transform, 50, DamageType.DirectHit, self.AI.player.transform.forward * 0.1f);
					missile.Fire();
				}
				else
				{
					GameObject obj1 = PoolManager.GetObject("MumukMissile", mumukPos[Random.Range(0, mumukPos.Count)].position, mumukPos[Random.Range(0, mumukPos.Count)].rotation);
					JangsungMumukMissile missile = obj1.GetComponent<JangsungMumukMissile>();
					missile.Init(mumukPos[0], self.AI.player.transform, 50, DamageType.NoHit);
					missile.Fire();
				}
			}
			else
			{
				int rand = UnityEngine.Random.Range(-1, 2);

				Vector3 dir = self.AI.player.transform.forward * rand * 0.1f;

				if (i % 5 == 0)
				{
					GameObject obj1 = PoolManager.GetObject("MumukMissile", mumukPos[0].position, mumukPos[0].rotation);
					JangsungMumukMissile missile = obj1.GetComponent<JangsungMumukMissile>();
					missile.Init(mumukPos[0], self.AI.player.transform, 50, DamageType.DirectHit, dir);
					missile.Fire();
				}
				else
				{
					GameObject obj1 = PoolManager.GetObject("MumukMissile", mumukPos[0].position, mumukPos[0].rotation);
					JangsungMumukMissile missile = obj1.GetComponent<JangsungMumukMissile>();
					missile.Init(mumukPos[0], self.AI.player.transform, 50, DamageType.NoHit, dir);
					missile.Fire();
				}
			}


		}

		/*
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
		*/
		OnAnimationEnd(evt);
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
	}

	public override void ResetAttackRange(int idx)
	{
	}

	public override void SetAttackRange(int idx)
	{
	}
}
