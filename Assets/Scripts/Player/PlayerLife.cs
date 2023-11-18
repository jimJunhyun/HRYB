using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : LifeModule 
{
	public override void Update()
	{
		base.Update();
		if (regenOn)
		{
			GameManager.instance.uiManager.yinYangUI.RefreshValues();
		}
	}

	public override void AddYYWXBase(YinyangWuXing data)
	{
		base.AddYYWXBase(data);
		GameManager.instance.uiManager.yinYangUI.RefreshValues();
		GameManager.instance.uiManager.wuXingUI.RefreshValues();
	}


	public override void OnDead()
	{
		base.OnDead();
		GetActor().anim.SetDieTrigger();
		Debug.Log("Player dead");
	}
}
