using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JangsungGirlLifeModule : LifeModule
{
	bool _isBarrier = false;
	public bool IsBarrier => _isBarrier;
	int _barrierNums = 0;

	public GameObject _barrierEffect;

	GameObject _objs;

	public void BarrierON(int a)
	{
		_objs = Instantiate(_barrierEffect, transform);
		_barrierNums = a;
		_isBarrier = true;
	}

	public override void AddYY(YinYang data, bool isNegatable = false, bool hit = true)
	{
		if(_isBarrier == false)
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
		else
		{

			if (hit == true)
			{
				_barrierNums--;
			}
			Debug.LogError($"보호막 : {_barrierNums}" );

			if(_barrierNums <=0)
			{
				_isBarrier = false;
				// 대충 베리어 이팩트 같은거 터지게 만들기
				Destroy(_objs);
				JangsungGirlAttack a = self.atk as JangsungGirlAttack;
				a.OnAnimationEnd();
			}
		}
	}

}
