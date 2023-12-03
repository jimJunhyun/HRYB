using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class Arrow : DamageObject
{
	public float disappearTime;

	public Vector3 massCenter;

	public float power = 60f;

	WaitForSeconds waitTillDisappear;

    Rigidbody rig;

	public override void OnTriggerEnter(Collider other)
	{
		if(!other.isTrigger)
		{
			if (other.TryGetComponent<LifeModule>(out LifeModule hit))
			{
				if(hit != GameManager.instance.pActor.life)
				{
					(GameManager.instance.pActor.cast as PlayerCast).NextComboMain(false);

				}
			}
			//Debug.Log(other.name);
			base.OnTriggerEnter(other);
			Returner();
		}
		
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

	public void Shoot()
	{
		rig.AddForce(power * transform.forward, ForceMode.Impulse);

	}

	IEnumerator DelReturn()
	{
		yield return waitTillDisappear;
		Returner();
		
	}

	void Returner()
	{
		//Debug.Log("RETURN");
		rig.velocity = Vector3.zero;
		StopAllCoroutines();
		PoolManager.ReturnObject(gameObject);
	}
}
