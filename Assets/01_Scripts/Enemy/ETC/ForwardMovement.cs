using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardMovement : ObjectAction
{
	[SerializeField] float _speed = 30;

	
	void Update()
	{
		if(_isFire)
			transform.position += transform.forward * _speed * Time.deltaTime;
	}
}
