using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraManager : MonoBehaviour
{
	bool blinded = false;
	Volume v;
	Camera main;

	private void Awake()
	{
		main = Camera.main;
		v = main.GetComponentInChildren<Volume>();
	}

	public void Blind(bool stat)
	{
		if(blinded != stat)
		{
			blinded = stat;
			v.gameObject.SetActive(stat);
		}
	}
}
