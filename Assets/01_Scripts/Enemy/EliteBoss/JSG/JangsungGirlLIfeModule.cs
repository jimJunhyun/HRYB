using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class JangsungGirlLifeModule : LifeModule
{
	bool _isBarrier = false;
	public bool IsBarrier => _isBarrier;
	int _barrierNums = 0;

	public GameObject _barrierEffect;

	GameObject _objs;

	Transform middle;

	public void BarrierON(int a)
	{
		_objs = Instantiate(_barrierEffect, transform);
		_barrierNums = a;
		_isBarrier = true;
	}

	public override void Awake()
	{
		base.Awake();
		if (transform.Find("Middle"))
		{
			middle = transform.Find("Middle");
		}
		else
		{
			middle = transform;
		}
	}


	protected override void DecreaseYY(float amt, YYInfo to, DamageChannel chn = DamageChannel.Normal)
	{
		float value = amt * adequity[((int)to)];
		yy.white.Value -= value;

		if (value > 0)
		{
			GameManager.instance.shower.GenerateDamageText(middle.position, value, to, chn);
		}
		if (isDead)
		{
			OnDead();
			StatusEffects.ApplyStat(GetActor(), GetActor(), StatEffID.Immune, 10);
		}
	}

	public override void DamageYY(YinYang data, DamageType type, float dur = 0, float tick = 0, Actor attacker = null, DamageChannel channel = DamageChannel.None)
	{
		if (_isBarrier == false)
		{
			base.DamageYY(data, type, dur, tick, attacker, channel);
		}
		else
		{

			if (DamageType.DirectHit == type)
			{
				_barrierNums--;
			}
			//Debug.LogError($"보호막 : {_barrierNums}");

			if (_barrierNums <= 0)
			{
				_isBarrier = false;
				// 대충 베리어 이팩트 같은거 터지게 만들기
				JangsungGirlAttack a = self.atk as JangsungGirlAttack;
				a.OnAnimationEnd();

				DeleteBarrier();
			}
		}
	}
	
	public override void DamageYY(float black, float white, DamageType type, float dur = 0, float tick = 0, Actor attacker = null, DamageChannel channel= DamageChannel.None)
	{
		Debug.Log("DD");
		if(_isBarrier == false)
		{
			base.DamageYY(black, white, type,dur,tick,attacker,channel);
		}
		else
		{

			if (DamageType.DirectHit == type)
			{
				_barrierNums--;
			}
			//Debug.LogError($"보호막 : {_barrierNums}" );

			if(_barrierNums <=0)
			{
				_isBarrier = false;
				// 대충 베리어 이팩트 같은거 터지게 만들기
				JangsungGirlAttack a = self.atk as JangsungGirlAttack;
				a.OnAnimationEnd();

				DeleteBarrier();
			}
		}
	}

	public void DeleteBarrier()
	{
		if(_objs)
		{
			Destroy(_objs);
		}
	}

}
