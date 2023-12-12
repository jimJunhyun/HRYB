using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;


[System.Serializable]
public struct StatusEffectApplyData
{
	public StatEffID id;
	public float power;
	public float duration;
	public StatusEffectApplyData(StatEffID eff, float pow, float dur)
	{
		id = eff;
		power = pow;
		duration = dur;
	}

	public override bool Equals(object obj)
	{
		if(obj != null && obj is StatusEffectApplyData d)
		{
			return d.id == id;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(id);
	}
}

public class Arrow : DamageObject
{
	public float disappearTime;

	public Vector3 massCenter;

	public float power = 60f;

	bool detectOn = true;

	Actor owner;

	WaitForSeconds waitTillDisappear;
	Coroutine c;

    Rigidbody rig;

	HashSet<StatusEffectApplyData> statData = new HashSet<StatusEffectApplyData>();

	public override void OnTriggerEnter(Collider other)
	{
		if(!other.isTrigger && detectOn)
		{
			if (other.TryGetComponent<LifeModule>(out LifeModule hit))
			{
				foreach (var item in statData)
				{
					StatusEffects.ApplyStat(hit.GetActor(), owner, item.id, item.duration, item.power);
				}
				GameManager.instance.ShakeCamFor(0.1f);
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

	public void SetOwner(Actor a)
	{
		owner = a;
	}

	public void ResetOwner()
	{
		owner = null;
	}

	public void Shoot()
	{

		SetDisappearTimer();
		rig.AddForce(power * transform.forward, ForceMode.Impulse);

	}

	public void AddStatusEffect(StatusEffectApplyData data)
	{
		statData.Add(data);
	}

	public void RemoveStatusEffect(StatEffID id)
	{
		statData.Remove(new StatusEffectApplyData(id, 0, 0));
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
		ResetOwner();
		statData.Clear();
		PoolManager.ReturnObject(gameObject);
	}
}
