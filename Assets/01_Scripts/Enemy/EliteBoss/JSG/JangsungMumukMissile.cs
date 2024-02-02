using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JangsungMumukMissile : MonoBehaviour
{
	Transform _target;
	float _speed;
	public bool _isFire = false;

	private void OnEnable()
	{
		_isFire = false;
	}

	public void Init(Transform pos, Transform target, float Speed)
	{
		transform.position = pos.position;
		transform.rotation = Quaternion.identity;
		_target = target;
		_isFire = false;
		_speed = Speed;
	}

	public void Fire()
	{
		_isFire = true;
	}

	private void Update()
	{
		if(_isFire)
		{
			transform.position += (transform.position - _target.position).normalized * _speed * Time.deltaTime;
			transform.LookAt(_target);
		}

	}


}
