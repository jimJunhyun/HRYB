using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AimPointCtrl : MonoBehaviour
{
	Image self;

	private void Awake()
	{
		self = GetComponent<Image>();
		Off();
	}

	public void Off()
	{
		self.enabled = false;
	}

	public void On()
	{
		self.enabled = true;
	}
}
