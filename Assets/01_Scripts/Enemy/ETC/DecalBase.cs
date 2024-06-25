using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DecalBase : MonoBehaviour
{

	public LayerMask _layer;

	public DecalProjector _start;
	public DecalProjector _end;

	Coroutine decal;
	float _currentTime = 0;
	float _endTime = 0;

	public void SetUpDecal(Vector3 vec, Quaternion qut, Vector3 decalStartSize, Vector3 decalEndSize)
	{
		RaycastHit ray;


		transform.localPosition += vec;
		if(Physics.Raycast(transform.position, new Vector3(0,1,0), out ray, 100, _layer))
		{
			transform.position = ray.point + new Vector3(0,-0.1f,0);
			Debug.LogError($"바닥 : {ray.point}");
		}


		transform.rotation = qut;
		transform.localEulerAngles = new Vector3(0,transform.localEulerAngles.y,0);


		_end.size = new Vector3(decalEndSize.x, 0.3f, decalEndSize.z);
		_start.size = new Vector3(decalStartSize.x,0.3f,decalStartSize.z);

		transform.SetParent(null);

	}

	public void SetUpDecalRay(Vector3 vec, Quaternion qut, Vector3 decalStartSize, Vector3 decalEndSize)
	{


		transform.position = vec;
		transform.rotation = qut;
		transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);

		_end.size = new Vector3(decalEndSize.x, 0.3f, decalEndSize.z);
		_start.size = new Vector3(decalStartSize.x, 0.3f, decalStartSize.z);

		transform.SetParent(null);

	}

	public void SetUpDecal(Transform _parent, Vector3 decalStartSize, Vector3 decalEndSize)
	{
		transform.parent = _parent;
		transform.localPosition = new Vector3(0, 0, 0) + new Vector3(0, -0.1f, 0);
		transform.localRotation = Quaternion.identity;
		_end.size = new Vector3(decalEndSize.x, 0.2f, decalEndSize.z);
		_start.size = new Vector3(decalStartSize.x, 0.2f, decalStartSize.z);
	}

	public void StartDecal(float _time)
	{
		_currentTime = 0;
		_endTime = _time;

		if (decal == null)
		{
			decal = StartCoroutine(DecalPush());

		}
	}

	public virtual IEnumerator DecalPush()
	{
		while (_currentTime <= _endTime) 
		{
			_start.size = Vector3.Lerp(_start.size, _end.size, _currentTime / _endTime);
			_currentTime += Time.deltaTime;
			yield return null;
		}
		decal = null;
		PoolManager.ReturnObject(gameObject);	
	}

}
