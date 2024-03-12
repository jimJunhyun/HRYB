using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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


	public override void DamageYY(float yin, float yang, DamageType type, float dur = 0, float tick = 0, Actor attacker = null)
	{
		if(_isBarrier == false)
		{
			YinYang data = new YinYang(yin, yang);
			switch (type)
			{
				case DamageType.DirectHit:
					if (!(isImmune))
					{
						DamageYYBase(data);
						GetActor().anim.SetHitTrigger();
						
						_hitEvent?.Invoke();
						
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
		else
		{

			if (DamageType.DirectHit == type)
			{
				_barrierNums--;
			}
			Debug.LogError($"보호막 : {_barrierNums}" );

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
