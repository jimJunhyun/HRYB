using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JangsungMumukMissile : MonoBehaviour
{
	Vector3 _target;
	float _speed;
	public bool _isFire = false;
	BoxColliderCast _cast;
	[SerializeField] LayerMask _enemy;
	[SerializeField] LayerMask _grond;
	Vector3 dir;

	private DamageType _channel;
	
	private void OnEnable()
	{
		_isFire = false;
		_cast = GetComponent<BoxColliderCast>();
	}

	public void Init(Transform pos, Transform target, float Speed, DamageType dmg, Vector3 damping = new Vector3())
	{
		if(_cast == null)
		{
			_cast = GetComponent<BoxColliderCast>();
		}

		transform.position = pos.position;
		transform.rotation = Quaternion.identity;
		_target = target.position;
		_isFire = false;
		_speed = Speed;
		dir = (_target - transform.position).normalized + damping;

		_channel = dmg;
	}

	public void Fire()
	{
		_isFire = true;

		if (PoolManager.GetObject("AttackDecal", transform).TryGetComponent<DecalBase>(out DecalBase _decal))
		{
			RaycastHit ray;
			if (Physics.Raycast(transform.position, dir.normalized, out ray, 100, 1 << 11))
			{
				_decal.SetUpDecalRay(ray.point + new Vector3(0, -0.1f, 0), transform.rotation, new Vector3(0, 0, 0), new Vector3(1.4f, 2, 1.4f));
				_decal.StartDecal(0.4f);
			}
		}

		_cast.Now(transform,(a)=>
		{
			if(1 << a.gameObject.layer == (int)_enemy)
			{
				//Debug.LogError("dd");
				a.DamageYY(0,10, _channel);
				CameraManager.instance.ShakeCamFor(0.3f, 0.8f, 0.8f);
			}
		});
		StartCoroutine(Returns());
	}

	private void Update()
	{
		if(_isFire)
		{
			transform.position += dir * _speed * Time.deltaTime;
			transform.LookAt(_target);
		}

		

	}


	IEnumerator Returns()
	{
		yield return new WaitForSeconds(5f);
		PoolManager.ReturnObject(gameObject);
	}
}
