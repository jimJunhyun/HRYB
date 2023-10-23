using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCast : CastModule
{
	private void Awake()
	{
		nameCastPair.Add("collect" , new Preparation(
		(self)=>
		{
			GameManager.instance.pinter.checkeds[GameManager.instance.pinter.curSel].InteractWith();
			GameManager.instance.pinter.Check();
		},
		1.0f));
	}
}
