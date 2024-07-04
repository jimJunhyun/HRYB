using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustDBCRotation : MonoBehaviour
{
	private Transform _hipsTr;
	public float Minus = -90.0f;

	private void Awake()
	{
		_hipsTr = transform.parent;
	}

	// Update is called once per frame
	void Update()
    {
        if(_hipsTr)
		{ //-90 0 -45 45 
			transform.localEulerAngles = new Vector3(_hipsTr.localEulerAngles.x - Minus, transform.localEulerAngles.y, transform.localEulerAngles.z);
		}
    }
}
