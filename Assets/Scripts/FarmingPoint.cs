using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmingPoint : MonoBehaviour, IInterable
{
	public float interTime = 1.0f;
	public bool isInterable = true;
	public bool isDestroyed = true;

	public bool IsInterable { get => isInterable; set => isInterable = value; }
	public float InterTime { get => interTime; set => interTime = value; }

	public void GlowOn()
	{
		
	}

	public void InteractWith()
	{
		Debug.Log(transform.name);
		if (isDestroyed)
		{
			Destroy(gameObject); //임시. 이후 풀링으로 변경.
		}
	}
}
