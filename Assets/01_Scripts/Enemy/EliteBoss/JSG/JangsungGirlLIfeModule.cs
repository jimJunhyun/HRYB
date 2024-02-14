using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JangsungGirlLIfeModule : LifeModule
{
	bool _isBarrier = false;
	public bool IsBarrier => _isBarrier;
	int _barrierNums = 0;

	public void BarrierON(int a)
	{
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

			if(_barrierNums <=0)
			{
				_isBarrier = false;
				// 대충 베리어 이팩트 같은거 터지게 만들기
				JangsungGirlAttack a = self.atk as JangsungGirlAttack;
				a.OnAnimationEnd();
			}
		}
	}

}
