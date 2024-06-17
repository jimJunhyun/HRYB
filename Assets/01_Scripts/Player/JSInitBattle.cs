using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

public class JSInitBattle : MonoBehaviour
{
	[SerializeField] private LifeModule _lifeOne;
	[SerializeField] private LifeModule _lifeTwo;
	
	[FormerlySerializedAs("_pl")] [SerializeField] private PlayableDirector _init;
	[SerializeField] private PlayableDirector _die;
	private bool _IsStart =false;
	private bool _isDead = false;

	private void Start()
	{
		gameObject.SetActive(false);
	}

	public void StartReseet()
	{
		_IsStart = false;
		SkyTimeManager.Instance.IsFixedOnDay = false;
	}

	private void Update()
	{
		if (_isDead == false && _lifeOne.isDead && _lifeTwo.isDead)
		{
			_isDead = true;
			_die.Play();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Player") && _IsStart ==false)
		{
			SkyTimeManager.Instance.IsFixedOnDay = true;
			_IsStart = true;
			_init.Play();
			GameManager.instance.audioPlayer.PlayBgm("JSPVPSound");
			RenderSettings.fogDensity = 0.02f;
		}
	}
}
