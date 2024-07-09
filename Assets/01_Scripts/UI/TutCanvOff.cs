using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutCanvOff : MonoBehaviour
{
    public void OffWind()
	{
		gameObject.SetActive(false);
		GameManager.instance.TimeFreezeSet(1);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			OffWind();
		}
	}
}
