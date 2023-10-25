using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCast : CastModule
{
	private void Start()
	{
		nameCastPair.Add("interact" , new Preparation(
		(self)=>
		{
			if(GameManager.instance.pinter.curFocused != null)
			{
				GameManager.instance.pinter.curFocused.InteractWith();
				GameManager.instance.pinter.Check();
			}
		},
		() =>
		{
			if(GameManager.instance.pinter.curFocused != null)
			{
				Debug.Log($"delSec : {GameManager.instance.pinter.curFocused.InterTime}");
				return GameManager.instance.pinter.curFocused.InterTime;
			}
			return 0;
		}));

		nameCastPair.Add("altInteract", new Preparation(
		(self) =>
		{
			if (GameManager.instance.pinter.curFocused != null && GameManager.instance.pinter.curFocused.AltInterable)
			{
				GameManager.instance.pinter.curFocused.AltInterWith();
				GameManager.instance.pinter.Check();
			}
		},
		() =>
		{
			if (GameManager.instance.pinter.curFocused != null)
			{
				Debug.Log($"delSec : {GameManager.instance.pinter.curFocused.InterTime}");
				return GameManager.instance.pinter.curFocused.InterTime;
			}
			return 0;
		}));
	}
}
