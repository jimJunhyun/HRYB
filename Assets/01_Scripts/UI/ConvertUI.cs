using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConvertUI : MonoBehaviour
{

	TMPro.TMP_Text[] guideKey;

	private void Start()
	{
		guideKey = /*GameObject.Find("KeyHelp").*/GetComponentsInChildren<TMP_Text>();
	}

	void Update()
    {
		if(GameManager.instance.pinven.stat == PlayerForm.Magic) //무기를 들고 있다면 키 가이드 변경
		{
			guideKey[0].text = "Q";
			guideKey[1].text = "E";
			guideKey[2].text = "R";
			guideKey[3].text = "F";
			guideKey[4].text = "C";
		}
		else
		{
			guideKey[0].text = "1";
			guideKey[1].text = "2";
			guideKey[2].text = "3";
			guideKey[3].text = "4";
			guideKey[4].text = "5";
		}
    }
}
