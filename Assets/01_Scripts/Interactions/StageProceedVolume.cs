using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageProceedVolume : MonoBehaviour
{
	public GameState to;
	public void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.layer == GameManager.PLAYERLAYER)
		{
			GameManager.instance.sManager.ProceedTo(to);
			Debug.Log("PROCEEDTO" + to.ToString());
		}
	}
}
