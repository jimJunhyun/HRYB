using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
	Actor act;
	private void OnTriggerEnter(Collider other)
	{
		if ((act = other.GetComponent<Actor>()))
		{
			if(act == GameManager.instance.pActor)
			{
				act.life.DamageYY(0, 99999999, DamageType.NoHit);
			}
		}
	}
}
