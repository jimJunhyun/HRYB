using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLifeModule : LifeModule
{
	[Header("Enemy")]
	public bool _isItGroge = false;
	public float _grogeInitValue = 100;
	public float _timePerGrogeAdd = 50;
	 float _currentGrogeValue;
	public float GetGrogeValue => _currentGrogeValue;
	bool _isGroge;
	public bool IsGroge => _isGroge;
	[Header("JunGI")]
	[SerializeField] bool _66PercentBlack = false;
	[SerializeField] bool _66PercentWhite = false;
	[SerializeField] bool _33PercentBlack = false;
	[SerializeField] bool _33PercentWhite = false;
	[SerializeField] bool _isDie = false;

	[Header("Items")]
	List<string> _dropItem = new();


	Transform middle;

	public override void Awake()
	{
		base.Awake();
		_currentGrogeValue = _grogeInitValue;

		_dieEvent += OutJeungGi;
		_dieEvent += DieEntity;

		if(transform.Find("Middle"))
		{
			middle = transform.Find("Middle");
		}
		else
		{
			middle = transform;
		}

	}
	public override void Update()
	{
		base.Update();

		if (_isItGroge && _isGroge)
		{
			_currentGrogeValue += _timePerGrogeAdd * Time.deltaTime;
			if(_currentGrogeValue >= _grogeInitValue)
			{
				_currentGrogeValue = _grogeInitValue;
				_isGroge = false;
			}
		}


	}
	
	public void DoGrogeDamage(float value)
	{
		if (_isItGroge == false || _isGroge)
			return;

		_currentGrogeValue -= value;
		if (_currentGrogeValue <= 0)
		{
			_isGroge = true;
			_currentGrogeValue = 0;
		}
	}

	public override void DamageYY(float black, float white, DamageType type, float dur = 0, float tick = 0, Actor attacker = null, DamageChannel channel = DamageChannel.None)
	{
		base.DamageYY(black, white, type, dur, tick, attacker, channel);
		OutJeungGi();
	}

	public override void DamageYY(YinYang data, DamageType type, float dur = 0, float tick = 0, Actor attacker = null, DamageChannel channel = DamageChannel.None)
	{

		base.DamageYY(data, type, dur, tick, attacker, channel);
		OutJeungGi();
	}

	protected override void DecreaseYY(float amt, YYInfo to, DamageChannel chn = DamageChannel.Normal)
	{
		float value = amt * adequity[((int)to)];
		Debug.Log($"{amt} * {adequity[((int)to)]} = {value}");
		yy.white.Value -= value;
		
		if(value > 0)
		{
			GameManager.instance.shower.GenerateDamageText(middle.position, value, to, chn);
		}
		if (isDead)
		{
			OnDead();
			StatusEffects.ApplyStat(GetActor(), GetActor(), StatEffID.Immune, 10);
		}
	}

	public void OutJeungGi()
	{
		if(yy.white.MaxValue * 0.66f > yy.white.Value && _66PercentWhite ==false)
		{
			_66PercentWhite = true;
			OutValue(yy.white.MaxValue * 0.0005f);
		}
		if (yy.white.MaxValue * 0.33f > yy.white.Value && _33PercentWhite == false)
		{
			_33PercentWhite = true;
			OutValue(yy.white.MaxValue * 0.0005f);
		}
		if(yy.white.Value <= 0 && _isDie == false)
		{
			_isDie = true;
			OutValue(yy.white.MaxValue * 0.0005f);
		}

		if (yy.black.MaxValue * 0.66f > yy.black.Value && _66PercentBlack == false)
		{
			_66PercentBlack = true;
			OutValue(yy.black.MaxValue * 0.0005f);
		}
		if (yy.black.MaxValue * 0.33f > yy.black.Value && _33PercentBlack == false)
		{
			_33PercentBlack = true;
			OutValue(yy.black.MaxValue * 0.0005f);
		}

		if (yy.black.Value <= 0 && _isDie ==false)
		{
			_isDie = true;
			OutValue(yy.black.MaxValue * 0.0005f);
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

		GameObject obj = PoolManager.GetObject("JunGI", transform.position + new Vector3(0,0.5f,0), Quaternion.identity);
		obj.transform.parent = null;

		if(t < 0)
		{
			t *= -1;
		}

		obj.GetComponent<JungGI>().Init(transform.position, t);



	}

	void OutHitEffect()
	{
		//if(_hit != "")
		//{
		//	EffectObject eff = PoolManager.GetEffect(_hit, _middle);
		//	eff.Begin();
		//}
	}
	public void AddComboDamage(float t)
	{
		// 나중에 엘리트몹 보스몹 나눠주기
		GameManager.instance.ComboRankManager.AddValue(t);
	}


	public void DieEntity()
	{
		// 유준현작업
	}
}


