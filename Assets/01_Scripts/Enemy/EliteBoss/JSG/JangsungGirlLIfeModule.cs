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

	[Header("JunGI")]
	[SerializeField] bool _66PercentBlack = false;
	[SerializeField] bool _66PercentWhite = false;
	[SerializeField] bool _33PercentBlack = false;
	[SerializeField] bool _33PercentWhite = false;
	[SerializeField] bool _isDie = false;

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
		OutJeungGi();
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

				GameManager.instance.loader.FadeStop();
				GameManager.instance.loader.FadeInOut("보호막이 파괴되었습니다.", 0.8f, 0.8f, 0.4f);

				JangsungGirlAttack a = self.atk as JangsungGirlAttack;
				a.OnAnimationEnd(null);

				DeleteBarrier();
			}
		}
	}
	
	public override void DamageYY(float black, float white, DamageType type, float dur = 0, float tick = 0, Actor attacker = null, DamageChannel channel= DamageChannel.None)
	{
		if(_isBarrier == false)
		{
			DamageYY(new YinYang(black, white), type, dur, tick, attacker, channel);
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
				a.OnAnimationEnd( null);

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

	public void OutJeungGi()
	{
		GameManager.instance.pActor.life.yy.black.Value += 4f;
		if (yy.white.MaxValue * 0.66f > yy.white.Value && _66PercentWhite == false)
		{
			_66PercentWhite = true;
			OutValue(yy.white.MaxValue * 0.004f);
		}
		if (yy.white.MaxValue * 0.33f > yy.white.Value && _33PercentWhite == false)
		{
			_33PercentWhite = true;
			OutValue(yy.white.MaxValue * 0.004f);
		}
		if (yy.white.Value <= 0 && _isDie == false)
		{
			_isDie = true;
			OutValue(yy.white.MaxValue * 0.004f);
		}

		if (yy.black.MaxValue * 0.66f > yy.black.Value && _66PercentBlack == false)
		{
			_66PercentBlack = true;
			OutValue(yy.black.MaxValue * 0.004f);
		}
		if (yy.black.MaxValue * 0.33f > yy.black.Value && _33PercentBlack == false)
		{
			_33PercentBlack = true;
			OutValue(yy.black.MaxValue * 0.004f);
		}

		if (yy.black.Value <= 0 && _isDie == false)
		{
			_isDie = true;
			OutValue(yy.black.MaxValue * 0.004f);
		}

		/*
		if (black > 0)
		{
			if (yy.black - black > 0)
			{
				OutValue(black / 10);
			}
			else if (yy.black - black <= 0)
			{
				OutValue(yy.black / 10);
			}
		}
		if (white > 0)
		{
			if (yy.white - white > 0)
			{
				OutValue(white / 10);
			}
			else if (yy.white - white <= 0)
			{
				OutValue(yy.white / 10);
			}
		}*/
	}

	void OutValue(float t)
	{

		GameObject obj = PoolManager.GetObject("JunGI", transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
		obj.transform.parent = null;

		if (t < 0)
		{
			t *= -1;
		}

		obj.GetComponent<JungGI>().Init(transform.position, t);



	}
}
