using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxDecal : DecalBase
{
	[SerializeField] public float _scaler = 1f;
	public override void BatchToMat()
	{
		
	}
	
	
	public override Vector3 EaseDecal()
	{
		return Vector3.Lerp(_start * _scaler, _end * _scaler, _currentTime / _endTime);
	}
}
