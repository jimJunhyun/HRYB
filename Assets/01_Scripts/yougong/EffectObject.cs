using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectObject : MonoBehaviour
{
	public Vector3 _originPosision;
	public Vector3 _originQuaternion;
	private ParticleSystem _particle;
	private bool _isPlay = false;
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
			EffectManager.ReturnObject(this);
		}
	}

	public void Begin()
	{
		_isPlay = true;
		
		Particle.Play();
	}

	public void Stop()
	{
		Particle.Stop();
		
		_isPlay = false;
		EffectManager.ReturnObject(this);
	}
	
}
