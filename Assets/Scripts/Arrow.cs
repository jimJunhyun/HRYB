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

	bool detectOn = true;

	WaitForSeconds waitTillDisappear;
	Coroutine c;

    Rigidbody rig;

	public override void OnTriggerEnter(Collider other)
	{
		if(!other.isTrigger && detectOn)
		{
			if (other.TryGetComponent<LifeModule>(out LifeModule hit))
			{
				if(hit != GameManager.instance.pActor.life)
				{
					(GameManager.instance.pActor.cast as PlayerCast).NextComboAt(SkillSlotInfo.LClick, false);

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
		
	}

	public void SetDisappearTimer()
	{
		if(c == null && gameObject.activeInHierarchy)
		{
			c = StartCoroutine(DelReturn());

		}
	}

	public void StopDisappearTimer()
	{
		if(c != null)
		{
			StopCoroutine(c);
		}
	}

	public void StopCheck()
	{
		detectOn = false;
	}

	public void ResumeCheck()
	{
		detectOn = true;
	}

	public void Shoot()
	{

		
		rig.AddForce(power * transform.forward, ForceMode.Impulse);

	}

	IEnumerator DelReturn()
	{
		yield return waitTillDisappear;
		Returner();
		c = null;
	}

	void Returner()
	{
		//Debug.Log("RETURN");
		rig.velocity = Vector3.zero;
		StopAllCoroutines();
		PoolManager.ReturnObject(gameObject);
	}
}
