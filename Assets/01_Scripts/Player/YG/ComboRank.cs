using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ComboRank : MonoBehaviour
{
	public const float none = 0;
	public float VeryLow = 1000;
	public float Low = 2000;
	public float Middle = 3000;
	public float High = 4000;
	public float VeryHigh = 5000;
	public float Z = 6000;

	public ComboRankSO _comboImage;

	float _value = 0;
	public float Value => _value;

	List<float> _listCombo = new();
	

	Coroutine _resetCount;

	PlayerLife _pl;

	PlayerLife Player
	{
		get
		{
			if(_pl == null )
			{
				_pl = GameManager.instance.player.GetComponent<PlayerLife>();
			}
			return _pl;
		}
	}

	Canvas _canvas;

	Image _front;
	Image _backGround;

	private void Awake()
	{
		_listCombo = new()
		{
			VeryHigh,
			High,	// 신
			Middle,	// 용
			Low,		// 귀
			VeryLow, // 호
			none		// 냥
		};

		_canvas = GameObject.Find("UICombo").GetComponent<Canvas>();
		var t = _canvas.gameObject.GetComponentsInChildren<Image>();

		for(int i =0; i < t.Length; i++)
		{
			if(t[i].name == "Front")
			{
				_front = t[i];
			}
			else if (t[i].name =="Background")
			{
				_backGround = t[i];
			}
		}
		_front.color = Color.clear;
		_backGround.color = Color.clear;
	}

	public void AddValue(float t)
	{
		_value += t;

		if(_value < 0 )
		{
			_value = 0;
			_front.color = Color.clear;
			_backGround.color = Color.clear;
			return;
		}

		if(_resetCount !=null)
		{
			StopCoroutine(_resetCount);
		}
		_resetCount = StartCoroutine(Calculate());


		
		
	}

	IEnumerator Calculate() 
	{
		yield return new WaitForSeconds(5f);


		if(_value >= VeryHigh)
		{
			for (int i = 0; i < 10; i++)
				OutValue(Player.yy.black.MaxValue * 0.2f);
		}
		else if(_value >= High)
		{
			for (int i = 0; i < 8; i++)
				OutValue(Player.yy.black.MaxValue * 0.1f);
		}
		else if(_value >= Middle)
		{
			for (int i = 0; i < 6; i++)
				OutValue(Player.yy.black.MaxValue * 0.1f);
		}
		else if(_value >= Low)
		{
			for (int i = 0; i < 4; i++)
				OutValue(Player.yy.black.MaxValue * 0.1f);
		}
		else if(_value >= VeryLow)
		{
			for (int i = 0; i < 2; i++)
				OutValue(Player.yy.black.MaxValue * 0.1f);
		}

		//for (int i = 0; i < 10; i++)
		//	OutValue(Player.initYinYang.black * 0.2f);

		_value = 0;
		_front.color = Color.clear;
		_backGround.color = Color.clear;

	}

	private void LateUpdate()
	{
		_value -= Time.deltaTime * 20f;
		if (_value <= 0)
		{
			_value = 0;
			return;
		}

		for (int i = 0; i < _listCombo.Count; i++)
		{
			if (_value >= _listCombo[i])
			{
				if(i != 0)
				{
					_front.fillAmount = (_value - _listCombo[i]) / (_listCombo[i-1] - _listCombo[i]);
				}
				else
				{
					_front.fillAmount = (_value - _listCombo[0]) / (Z - _listCombo[0]);
				}
					//Debug.LogError($"Combo Valeu : {_value}/ {((int)_listCombo[i - 1])} = {_value / ((int)_listCombo[i-1])}");

				if (i < _listCombo.Count - 1)
				{
					_front.sprite = _comboImage.ComboRankImage[i];
					_backGround.sprite = _comboImage.ComboRankImageMask[i];
					_front.color = Color.white;
					_backGround.color = Color.white;
				}
				else
				{
					_front.color = Color.clear;
					_backGround.color = Color.clear;
				}
				break;
			}
		}

	}

	void OutValue(float t)
	{

		GameObject obj = PoolManager.GetObject("JunGI", Player.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
		obj.transform.parent = null;
		if (t < 0)
		{
			t *= -1;
		}
		obj.GetComponent<JungGI>().Init(Player.transform.position, t);
	}

}
