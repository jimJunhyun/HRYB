using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : LifeModule 
{
	public override void OnDead()
	{
		Debug.Log("Player dead");
	}
}
