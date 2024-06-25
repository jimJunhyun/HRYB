using Unity.Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillProduction : MonoBehaviour
{
	CinemachineVirtualCamera _cam;
	public CinemachineBasicMultiChannelPerlin _shakes;

	Coroutine _co;

	private void Awake()
	{
		_cam = GetComponentInChildren<CinemachineVirtualCamera>();
		_shakes = _cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
		
	}

	public void Begin(GameObject Follow = null, GameObject See = null)
	{
		_cam.Priority = 100;
		GameManager.instance.camManager.RegisterSkillCam(this);
		if(Follow != null)
			_cam.Follow = Follow.transform;
		if(See != null)
			_cam.LookAt = See.transform;
	}

	private void Update()
	{
		_shakes.AmplitudeGain = CameraManager.instance.camShakers[0].AmplitudeGain;
		_shakes.FrequencyGain = CameraManager.instance.camShakers[0].FrequencyGain;
	}

	public void End()
	{
		_cam.Priority = 0;
		if(_co == null)
		_co = StartCoroutine(ReturnLate());
	}


	IEnumerator ReturnLate()
	{
		yield return new WaitForSeconds(3f);
		_co = null;
		PoolManager.ReturnObject(gameObject);
	}



}
