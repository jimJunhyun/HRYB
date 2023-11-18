using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InterProcessUI : MonoBehaviour
{
    public Slider gauge;
	bool isOn = true;

	private void Awake()
	{
		gauge = GetComponent<Slider>();
	}

	public void SetGaugeValue(float v)
	{
		gauge.value = v;
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
