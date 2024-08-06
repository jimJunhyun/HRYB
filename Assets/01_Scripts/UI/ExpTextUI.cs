using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExpTextUI : MonoBehaviour
{
	TextMeshProUGUI txt;

	private void Start()
	{
		DoRefresh();
	}
	public void DoRefresh()
	{
		if(txt == null)
		{
			txt = GetComponent<TextMeshProUGUI>();
		}

		
		bool b = GameManager.GetGlobalSB(out System.Text.StringBuilder sb);
		sb.Append("<#ffffff>내공 : </color><#00dd00>");
		sb.Append(GameManager.instance.pinven.currentExp);
		sb.Append("</color>\n");
		txt.text = sb.ToString();
		GameManager.ReturnGlobalSB(b);
	}
}
