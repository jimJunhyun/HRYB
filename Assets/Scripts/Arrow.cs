using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : DamageObject
{
	public float disappearTime;

	public Vector3 massCenter;

	WaitForSeconds waitTillDisappear;

    Rigidbody rig;

	public override void OnTriggerEnter(Collider other)
	{
		base.OnTriggerEnter(other);
		Returner();
	}

	private void Awake()
	{
		rig = GetComponent<Rigidbody>();
		waitTillDisappear = new WaitForSeconds(disappearTime);
	}

	private void Update()
	{
		if(rig.velocity.sqrMagnitude != 0)
		{
			transform.rotation = Quaternion.LookRotation(rig.velocity);

		}
	}

	private void OnEnable()
	{
		StartCoroutine(DelReturn());
	}

	public void Shoot(float val)
	{
		rig.AddForce(val * transform.forward, ForceMode.Impulse);

	}

	IEnumerator DelReturn()
	{
		yield return waitTillDisappear;
		Returner();
		
	}

	void Returner()
	{
		rig.velocity = Vector3.zero;
		StopAllCoroutines();
		PoolManager.ReturnObject(gameObject);
	}
}
