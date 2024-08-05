using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatTxtUI : MonoBehaviour
{
	TextMeshProUGUI txt;

	private void Start()
	{
		DoRefresh();
	}
	public void DoRefresh()
	{
		if (txt == null)
		{
			txt = GetComponent<TextMeshProUGUI>();
		}


		bool b = GameManager.GetGlobalSB(out System.Text.StringBuilder sb);
		sb.Append("<#000000>체력 : </color><#00dd00>");
		sb.Append(GameManager.instance.pActor.life.yy.white.MaxValue);
		sb.Append("</color>\n");
		sb.Append("<#000000>기력 : </color><#00dd00>");
		sb.Append(GameManager.instance.pActor.life.yy.black.MaxValue);
		sb.Append("</color>\n");
		sb.Append("<#000000>힘 : </color><#00dd00>");
		sb.Append(GameManager.instance.pActor.atk.Damage.white.MaxValue);
		sb.Append("</color>\n");
		sb.Append("<#000000>정신 : </color><#00dd00>");
		sb.Append(GameManager.instance.pActor.atk.Damage.black.MaxValue);
		sb.Append("</color>\n");
		sb.Append("<#000000>속도 : </color><#00dd00>");
		sb.Append(GameManager.instance.pActor.move.Speed);
		sb.Append("</color>\n");
		txt.text = sb.ToString();
		GameManager.ReturnGlobalSB(b);
	}
}
