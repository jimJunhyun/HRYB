using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public enum ArrowMode
{
	Normal,
	Homing,

}

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

	public float maxSpeed = 50f;

	Transform target;

	ArrowMode mode = ArrowMode.Normal;
	public float homingPower = 60f;

	bool fired = false;
	bool detectOn = true;

	Actor owner;

	WaitForSeconds waitTillDisappear;
	Coroutine c;

    Rigidbody rig;

	HashSet<StatusEffectApplyData> statData = new HashSet<StatusEffectApplyData>();
	Action<Vector3> hitEffData;

	public override void OnTriggerEnter(Collider other)
	{
		if(!other.isTrigger && detectOn)
		{
			if (other.TryGetComponent<LifeModule>(out LifeModule hit) && hit.GetActor() != owner)
			{
				
				if(hitEffData != null)
				{
					Vector3 hitPos = other.ClosestPointOnBounds(transform.position);
					//차격 지점을 알아내야만 함.
					hitEffData?.Invoke(hitPos);
					//타격 위치에 이펙트
				}
				foreach (var item in statData)
				{
					StatusEffects.ApplyStat(hit.GetActor(), owner, item.id, item.duration, item.power);
				}
				base.OnTriggerEnter(other);
				CameraManager.instance.ShakeCamFor(0.1f);
			}
			Debug.Log(other.name + " 과 충돌");
			if(other.transform != owner)
			{
				Returner();
			}
		}
		
	}

	public override void Damage(LifeModule to)
	{
		if(owner.move is PlayerMove)
		{
			to.DamageYY(yy, DamageType.DirectHit, 0, 0, owner);
		}
		else
		{
			to.DamageYY(yy, DamageType.DirectHit);
		}
	}

	private void Awake()
	{
		rig = GetComponent<Rigidbody>();
		waitTillDisappear = new WaitForSeconds(disappearTime);
	}

	private void Update()
	{
		switch (mode)
		{
			case ArrowMode.Normal:
				if (rig.velocity.sqrMagnitude != 0)
				{
					transform.rotation = Quaternion.LookRotation(rig.velocity);
				}
				break;
			case ArrowMode.Homing:
				if (fired)
				{
					if (target != null)
					{
						Vector3 dir = (target.position - transform.position);
						transform.forward = dir.normalized;	
						rig.AddForce(transform.forward * homingPower, ForceMode.VelocityChange);
					}
					//rig.AddForce(transform.forward * power, ForceMode.Force);
				}
				break;
			default:
				break;
		}
		Vector3 vel = Vector3.ClampMagnitude(rig.velocity, maxSpeed);
		rig.velocity = vel;

	}

	private void OnEnable()
	{
		
	}

	private void OnDisable()
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

	public void SetHitEff(Action<Vector3> onHit)
	{

		hitEffData = (onHit);
	}

	public void SetTarget(Transform targ)
	{
		if (targ)
		{
			target = targ.Find("Middle");
			if (!target)
			{
				target = targ;
			}
		}
		
	}

	public void Shoot(ArrowMode mode)
	{
		SetDisappearTimer();
		
		if(mode == ArrowMode.Homing && target != null)
		{
			rig.AddForce(transform.forward * power * 0.1f, ForceMode.Impulse);
		}
		else
		{
			rig.AddForce(power * transform.forward, ForceMode.Impulse);
		}

		this.mode = mode;
		fired = true;
	}

	public void AddStatusEffect(StatusEffectApplyData data)
	{
		Debug.Log("STAT ADDED : " + data.id.ToString());
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
		Debug.Log("RETURN");
		rig.velocity = Vector3.zero;
		StopAllCoroutines();
		ResetOwner();
		statData.Clear();
		PoolManager.ReturnAllChilds(gameObject);
		mode = ArrowMode.Normal;
		fired = false;
		target = null;
	}

	
}
