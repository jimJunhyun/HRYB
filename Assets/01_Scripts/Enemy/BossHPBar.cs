using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHPBar : MonoBehaviour
{
	[SerializeField] Image _hpBar;
	[SerializeField] Actor _lifeModule;



	public void Init(Actor lf)
	{
		_lifeModule = lf;
	}

	void Update()
    {
		if(_lifeModule != null && _lifeModule.life.yy != null)
		{
			Debug.Log(_lifeModule.life.yy.white / _lifeModule.life.initYinYang.white);
			_hpBar.fillAmount = _lifeModule.life.yy.white / _lifeModule.life.initYinYang.white;
		}
    }
}
