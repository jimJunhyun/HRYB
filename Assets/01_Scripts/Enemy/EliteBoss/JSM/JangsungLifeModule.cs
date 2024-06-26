using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class JangsungLifeModule : LifeModule
{
	bool _isBarrier = false;
	public bool IsBarrier => _isBarrier;

	public GameObject _barrierEffect;
	public Transform _visualPos;
	GameObject _objs;
	
	Transform middle;
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

	public void BarrierON()
	{
		_objs = Instantiate(_barrierEffect, _visualPos);
		_isBarrier = true;
	}

	public void BarrierOff()
	{
		_isBarrier = false;
		if(_objs != null )
		{
			Destroy(_objs);
		}
	}
	public override void DamageYY(YinYang data, DamageType type, float dur = 0, float tick = 0, Actor attacker = null, DamageChannel channel = DamageChannel.None)
	{
		if (_isBarrier == false)
		{
			base.DamageYY(data, type, dur, tick, attacker, channel);
		}
	}

	public override void DamageYY(float black, float white, DamageType type, float dur = 0, float tick = 0, Actor attacker = null, DamageChannel channel= DamageChannel.None)
	{
		if (_isBarrier == false)
		{
			base.DamageYY(black, white, type, dur, tick, attacker, channel);
		}
	}
}

