using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : DamageObject
{

    Rigidbody rig;

	public override void OnTriggerEnter(Collider other)
	{
		base.OnTriggerEnter(other);
		Destroy(gameObject);
	}
	private void OnEnable()
	{
		rig = GetComponent<Rigidbody>();
	}

	public void Shoot(float val)
	{
		rig.AddForce(val * transform.forward, ForceMode.Impulse);
	}
}
