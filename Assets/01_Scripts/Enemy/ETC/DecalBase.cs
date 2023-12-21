using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DecalBase : MonoBehaviour
{
	[SerializeField] private Transform _obj;
	protected Vector3 _start = new Vector3(0,0,0);
	protected Vector3 _end = new Vector3(1,1,1);
	protected void Awake()
	{
		BatchToMat();
	}
	public abstract void BatchToMat();
	private Coroutine decal;
	private bool _isStop = true;

	protected float _currentTime = 0;
	protected float _endTime = 0;

	public void SetUpDecal(Vector3 vec, Quaternion qut, Vector3 decalSize, Vector3 decalStartSize, Vector3 decalEndSize)
	{
		transform.localPosition += vec + new Vector3(0,0.05f,0);
		transform.rotation = qut;
		transform.localScale = decalSize;
		_start = decalStartSize;
		_end = decalEndSize;

		transform.SetParent(null);
		
		_isStop = true;
	}

	public void SetUpDecal(Transform _parent, Vector3 decalStartSize, Vector3 decalEndSize)
	{
		transform.parent = _parent;
		transform.localPosition = new Vector3(0, 0.05f, 0);
		_start = decalStartSize;
		_end = decalEndSize;
		
		_isStop = true;
	}

	public void StartDecal(float _time)
	{
		_currentTime = 0;
		_endTime = _time;
		
		if(decal == null)
		{
			decal = StartCoroutine(DecalPush());
			
		}
		
	}

	IEnumerator DecalPush()
	{
		_isStop = false;
		yield return new WaitForSeconds(_endTime);
		_isStop = true;
		decal = null;
		PoolManager.ReturnObject(gameObject);
	}
	
	public virtual Vector3 EaseDecal()
	{
		return Vector3.Lerp(_start, _end, _currentTime / _endTime);
	}

	protected void Update()
	{
		if (_isStop)
			return;

		_currentTime += Time.deltaTime;
		_obj.localScale = EaseDecal();
	}

	public void StopDecal()
	{
		_isStop = true;
		if(decal != null)
			StopCoroutine(decal);
		decal = null;
		PoolManager.ReturnObject(gameObject);
	}
	
	
}
