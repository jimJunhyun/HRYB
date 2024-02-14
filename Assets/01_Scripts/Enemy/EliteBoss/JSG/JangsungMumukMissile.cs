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

	private void OnEnable()
	{
		_isFire = false;
		_cast = GetComponent<BoxColliderCast>();
	}

	public void Init(Transform pos, Transform target, float Speed)
	{
		transform.position = pos.position;
		transform.rotation = Quaternion.identity;
		_target = target.position;
		_isFire = false;
		_speed = Speed;
	}

	public void Fire()
	{
		_isFire = true;
		_cast.Now((a)=>
		{
			if(a.gameObject.layer == _enemy)
			{
				a.AddYY(new YinYang(10, 0), true);

				CameraManager.instance.ShakeCamFor(0.3f);
			}
		});
	}

	private void Update()
	{
		if(_isFire)
		{
			transform.position += (transform.position - _target).normalized * _speed * Time.deltaTime;
			transform.LookAt(_target);
		}

	}


}
