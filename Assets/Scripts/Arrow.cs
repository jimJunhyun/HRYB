using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    Rigidbody rig;
	private void Awake()
	{
		rig = GetComponent<Rigidbody>();
	}

	public void Shoot(float val)
	{
		rig.AddForce(val * transform.forward, ForceMode.Impulse);
	}
}
