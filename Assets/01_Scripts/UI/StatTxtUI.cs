using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatTxtUI : MonoBehaviour
{
	public StatUpgradeType indicatingStat;
	TextMeshProUGUI nameTxt;
	TextMeshProUGUI amtTxt;

	private void Start()
	{
		DoRefresh();
	}
	public void DoRefresh()
	{
		if (nameTxt == null)
		{
			nameTxt = transform.Find("StatName").GetComponent<TextMeshProUGUI>();
		}
		if (amtTxt == null)
		{
			amtTxt = transform.Find("StatAmt").GetComponent<TextMeshProUGUI>();
		}

		bool b = GameManager.GetGlobalSB(out System.Text.StringBuilder sb);
		sb.Append("<sprite=");
		sb.Append(((int)indicatingStat));
		sb.Append(">");
		sb.Append(NodeUtility.ToStringKorean(indicatingStat));
		nameTxt.text = sb.ToString();
		GameManager.ReturnGlobalSB(b);
		switch (indicatingStat)
		{
			case StatUpgradeType.White:
				amtTxt.text = GameManager.instance.pActor.life.yy.white.ToString();
				break;
			case StatUpgradeType.Black:
				amtTxt.text = GameManager.instance.pActor.life.yy.black.ToString();
				break;
			case StatUpgradeType.WhiteAtk:
				amtTxt.text = GameManager.instance.pActor.atk.Damage.white.MaxValue.ToString();
				break;
			case StatUpgradeType.BlackAtk:
				amtTxt.text = GameManager.instance.pActor.atk.Damage.black.MaxValue.ToString();
				break;
			case StatUpgradeType.MoveSpeed:
				amtTxt.text = GameManager.instance.pActor.move.Speed.ToString();
				break;
			case StatUpgradeType.CooldownRdc:
				amtTxt.text = (1 - (GameManager.instance.pActor.cast as PlayerCast).cooldownModuleStat.Speed).ToString();
				break;
			default:
				break;
		}
	}
}
