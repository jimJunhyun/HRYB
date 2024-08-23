using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutCanvOff : MonoBehaviour
{
    public void OffWind(bool esc)
	{
		gameObject.SetActive(false);
		if (!esc)
		{
			GameManager.instance.TimeFreezeSet(1);
			GameManager.instance.LockCursor();
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			OffWind(true);
		}
	}
}
