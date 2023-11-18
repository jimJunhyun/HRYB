using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InterPrevUI : MonoBehaviour
{
    public TextMeshProUGUI text;
	public Image fill;
	bool isOn = true;

	private void Awake()
	{
		text = GetComponentInChildren<TextMeshProUGUI>();
		fill = transform.Find("Fill").GetComponent<Image>();
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
