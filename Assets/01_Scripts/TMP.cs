using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;

public class TMP : MonoBehaviour
{
	TextMeshProUGUI ugui;
	private void Awake()
	{
		ugui = GetComponent<TextMeshProUGUI>();
	}
	public void Write(string st)
    {
		ugui.text += st;
    }
}
