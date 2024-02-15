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

	public override void AddYY(YinYang data, bool isNegatable = false, bool hit = true)
	{
		if (_isBarrier == false)
		{
			if (!(isNegatable && isImmune))
			{
				AddYYBase(data);
				GetActor().anim.SetHitTrigger();
				StatusEffects.ApplyStat(GetActor(), GetActor(), StatEffID.Immune, IMMUNETIME);
			}

			if (hit == true)
			{
				_hitEvent?.Invoke();
			}
		}
	}
}

