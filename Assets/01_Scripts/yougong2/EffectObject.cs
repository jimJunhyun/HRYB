using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class EffectObject : MonoBehaviour
{
	public Vector3 _originPosision;
	public Vector3 _originQuaternion;
	private ParticleSystem _particle;
	private bool _isPlay = false;
	[Description("Infinity = -1")]
	public float _durationTime = -1.0f;
	private ParticleSystem Particle
	{
		get
		{
			if (_particle == null)
			{
				_particle = GetComponent<ParticleSystem>();
			}
			return _particle;
		}

	}

	private void Update()
	{

		if (Particle.isPlaying == false && _isPlay==true)
		{
			_isPlay = false;
			PoolManager.ReturnObject(gameObject);
		}
	}

	public void Begin()
	{
		_isPlay = true;
		
		transform.localPosition = _originPosision;
		transform.localEulerAngles = _originQuaternion;
		
		
		
		Particle.Play();
	}

	public IEnumerator LifeDuration()
	{
		yield return new WaitForSeconds(_durationTime);
		PoolManager.ReturnObject(this.gameObject);
	}

	public void Stop()
	{
		Particle.Stop();
		
		_isPlay = false;
		PoolManager.ReturnObject(gameObject);
	}
	
}
