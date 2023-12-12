using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hookables : MonoBehaviour
{
    GameObject rope;
	private void Awake()
	{
		 rope = transform.Find("Rope").gameObject;
		ResetRope();
	}

	public void SetRope()
	{
		rope.SetActive(true);
	}

	public void ResetRope()
	{
		rope.SetActive(false);
	}
}
