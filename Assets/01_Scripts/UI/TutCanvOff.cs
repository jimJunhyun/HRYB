using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutCanvOff : MonoBehaviour
{
    public void OffWind(bool esc)
	{
		gameObject.SetActive(false);
		GameManager.instance.TimeFreezeSet(1);
		if (!esc)
		{
			GameManager.instance.LockCursor();
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			OffWind(true);
			GameManager.instance.TimeFreezeSet(0);
		}
	}
}
