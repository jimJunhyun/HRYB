using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InterPrevUI : MonoBehaviour
{
    public TextMeshProUGUI text;
    public TextMeshProUGUI descText;
    public TextMeshProUGUI descTextAlt;
	public Image fill;
	bool isOn = true;

	private void Awake()
	{
		text = GetComponentsInChildren<TextMeshProUGUI>()[0];
		descText = GetComponentsInChildren<TextMeshProUGUI>()[1];
		descTextAlt = GetComponentsInChildren<TextMeshProUGUI>()[2];
		fill = transform.Find("Fill").GetComponent<Image>();
		SetGaugeValue(0);
	}
	
	public void SetDescTxt(string txt)
	{
		if (txt == null)
		{
			descText.text = "";
		}
		else
		{

			descText.text = $"누르기 : {txt}";
		}
	}

	public void SetDescAltTxt(string txt)
	{
		if (txt == null)
		{
			descTextAlt.text = "";
		}
		else
		{

			descTextAlt.text = $"홀드 : {txt}";
		}
	}

	public void SetKeyInfoTxt(string txt)
	{
		text.text = txt;
	}

	public void SetGaugeValue(float val)
	{
		fill.fillAmount = val;
	}

	public void On()
	{
		if (!isOn)
		{
			isOn = true;
			gameObject.SetActive(true);
		}
	}

	public void Off()
	{
		if (isOn)
		{
			isOn = false;
			gameObject.SetActive(false);
		}
	}
}
