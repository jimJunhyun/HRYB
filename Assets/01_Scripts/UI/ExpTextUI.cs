using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExpTextUI : MonoBehaviour
{
	TextMeshProUGUI txt;
    public void DoRefresh()
	{
		if(txt == null)
		{
			txt = GetComponent<TextMeshProUGUI>();
		}

		
		bool b = GameManager.GetGlobalSB(out System.Text.StringBuilder sb);
		sb.Append("<#000000>내공 : </color><#555555>");
		sb.Append(GameManager.instance.pinven.currentExp);
		sb.Append("</color>");
		GameManager.ReturnGlobalSB(b);
	}
}
