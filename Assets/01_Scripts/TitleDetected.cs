using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleDetected : MonoBehaviour
{
	public string text;
	

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.layer ==  GameManager.PLAYERLAYER || other.gameObject.layer == GameManager.PLAYERATTACKLAYER)
		{
			Debug.LogError("으악 사람이다!");
			GameManager.instance.loader.FadeInOut(text, 0.5f);
			
		}
	}
}
