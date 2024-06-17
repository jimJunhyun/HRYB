using OccaSoftware.SuperSimpleSkybox.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyTimeManager : Singleton<SkyTimeManager>
{
	private Sun sun;
	private Moon moon;

	private bool isFixedOnDay = false;

	public bool IsFixedOnDay
	{
		get => isFixedOnDay;

		set
		{
			isFixedOnDay = value;

			sun.IsFixedOnDay = value;
			moon.IsFixedOnDay = value;
		}
	}

	private bool isRotating = true;

	public bool IsRotating
	{
		get => isRotating;

		set
		{
			isRotating = value;

			sun.IsRotating = value;
			moon.IsRotating = value;
		}
	}

	private void Awake()
	{
		sun = FindObjectOfType<Sun>();
		moon = FindObjectOfType<Moon>();
	}
}
