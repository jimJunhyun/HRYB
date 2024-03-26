using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JangsungLifeModule : LifeModule
{
	bool _isBarrier = false;
	public bool IsBarrier => _isBarrier;

	public GameObject _barrierEffect;
	public Transform _visualPos;
	GameObject _objs;

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
		DamageYY(data.black, data.white, type, dur,tick,attacker,channel);
	}

	public override void DamageYY(float black, float white, DamageType type, float dur = 0, float tick = 0, Actor attacker = null, DamageChannel channel= DamageChannel.None)
	{
		if (_isBarrier == false)
		{
			YinYang data = new YinYang(black, white);
			switch (type)
			{
				case DamageType.DirectHit:
					if (!(isImmune))
					{
						DamageYYBase(data);
						
						_hitEvent?.Invoke();
						
						GetActor().anim.SetHitTrigger();
						StatusEffects.ApplyStat(GetActor(), GetActor(), StatEffID.Immune, IMMUNETIME);
					}
					break;
				case DamageType.DotDamage:
				case DamageType.Continuous:
					StartCoroutine(DelDmgYYWX(data, dur, tick, type));
					break;
				case DamageType.NoEvadeHit:
					DamageYYBase(data);
					GetActor().anim.SetHitTrigger();
					StatusEffects.ApplyStat(GetActor(), GetActor(), StatEffID.Immune, IMMUNETIME);
					break;
				default:
					break;
			}
		}
	}
}

