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

	public override void AddYYBase(YinYang data)
	{
		base.AddYYBase(data);
		GameManager.instance.uiManager.yinYangUI.RefreshValues();
	}


	public override void OnDead()
	{
		base.OnDead();
		//GetActor().anim.SetDieTrigger();
		GetActor().move.moveDir = Vector3.zero;
		GetActor().move.forceDir = Vector3.zero;
		(GetActor().move as PlayerMove).ctrl.center = Vector3.up;
		(GetActor().move as PlayerMove).ctrl.height = 1;
		GetActor().Respawn();

		transform.position = Vector3.zero;
		Debug.Log("Player dead");
	}
}
