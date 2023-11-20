using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightModule : Module
{
	public float initSightRange;

	[HideInInspector]
    public float sightRange;


	public float GetSightRange()
	{
		return sightRange;
	}

	public override void ResetStatus()
	{
		base.ResetStatus();
		sightRange = initSightRange;
	}
}
