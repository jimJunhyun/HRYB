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
			if((GetActor().sight as PlayerInter).curFocused != null)
			{
				//GameManager.instance.uiManager.debugText.text += $"  FUNC EXECUTING {(GetActor().sight as PlayerInter).curFocused}...  ";
				(GetActor().sight as PlayerInter).curFocused.InteractWith();
				(GetActor().sight as PlayerInter).Check();
				
			}
		},
		() =>
		{
			if((GetActor().sight as PlayerInter).curFocused != null)
			{
				//Debug.Log($"delSec : {GameManager.instance.pinter.curFocused.InterTime}");
				float t = (GetActor().sight as PlayerInter).curFocused.InterTime;
				(GetActor().anim as PlayerAnim).SetInterSpeed((1 / t) * castMod);
				return t;
			}
			return 0;
		}));

		nameCastPair.Add("altInteract", new Preparation(
		(self) =>
		{
			if ((GetActor().sight as PlayerInter).curFocused != null && (GetActor().sight as PlayerInter).curFocused.AltInterable)
			{
				(GetActor().sight as PlayerInter).curFocused.AltInterWith();
				(GetActor().sight as PlayerInter).Check();
			}
		},
		() =>
		{
			if ((GetActor().sight as PlayerInter).curFocused != null)
			{
				float t = (GetActor().sight as PlayerInter).curFocused.InterTime;
				(GetActor().anim as PlayerAnim).SetInterSpeed((1 / t) * castMod);
				return t;
			}
			return 0;
		}));
	}

	public override void CastCancel()
	{
		base.CastCancel();
		GameManager.instance.uiManager.interingUI.Off();
	}

	protected override IEnumerator DelCast(Preparation p)
	{
		GameManager.instance.pinp.DeactivateInput();
		GameManager.instance.uiManager.interingUI.On();
		float t = 0;
		float waitSec = p.Timefunc();
		while (waitSec * castMod > t)
		{
			t += Time.deltaTime;
			GameManager.instance.uiManager.interingUI.SetGaugeValue(t / (waitSec * castMod));
			yield return null;
		}
		p.onPrepComp?.Invoke(transform);
		ongoing = null;
		curName = null;
		GameManager.instance.uiManager.interingUI.Off();
		GameManager.instance.pinp.ActivateInput();
	}
}
