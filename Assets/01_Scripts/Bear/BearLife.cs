using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearLife : LifeModule
{
	public override void OnDead()
	{
		Destroy(gameObject); //tmp
		//base.OnDead();
	}
}
