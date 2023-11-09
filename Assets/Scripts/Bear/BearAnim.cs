using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearAnim : AnimModule
{
	private void Update()
	{
		if (!GetActor().move.idling)
		{
			SetMoveState(1);
		}
		else
		{
			SetMoveState(0);
		}
	}
}
