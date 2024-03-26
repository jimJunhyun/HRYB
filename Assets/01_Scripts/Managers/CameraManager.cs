using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraManager : MonoBehaviour
{
	List<CinemachineBasicMultiChannelPerlin> camShakers = new List<CinemachineBasicMultiChannelPerlin>();
	
	public CamStatus curCamStat;
	
	public const int FORWARDCAM = 20;
	public const int BACKWARDCAM = 10;
	
	bool blinded = false;
	Volume v;
	Camera _main;
	
	public CinemachineFreeLook pCam;
	public CinemachineVirtualCamera aimCam;

	public static CameraManager instance;
	
	public Camera MainCam
	{
		get
		{
			if (_main == null)
			{
				_main = Camera.main;
			}
			
			return _main;
		}
	}

	private void Awake()
	{
		instance = this;
		_main = Camera.main;
		
		pCam = GetComponent<CinemachineFreeLook>();
		aimCam = GameObject.Find("AimCam").GetComponent<CinemachineVirtualCamera>();
		
		SwitchTo(CamStatus.Freelook);
		for (int i = 0; i < 3; i++)
		{
			camShakers.Add(pCam.GetRig(i).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>());
		}

		v = _main.GetComponentInChildren<Volume>();
		
		for (int i = 0; i < camShakers.Count; i++)
		{
			camShakers[i].m_AmplitudeGain = 0;
			camShakers[i].m_FrequencyGain = 0;
		}
	}
	
	public void SwitchTo(CamStatus stat)
	{
		curCamStat = stat;
		switch (stat)
		{
			case CamStatus.Freelook:
				pCam.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
				pCam.m_XAxis.m_Wrap = true;
				pCam.Priority = FORWARDCAM;
				aimCam.Priority = BACKWARDCAM;
				break;
			case CamStatus.Aim:
				aimCam.Priority = FORWARDCAM;
				pCam.Priority = BACKWARDCAM;
				break;
			case CamStatus.Locked:
				pCam.m_BindingMode = CinemachineTransposer.BindingMode.LockToTargetWithWorldUp;
				pCam.m_XAxis.m_Wrap = false;
				pCam.Priority = FORWARDCAM;
				aimCam.Priority = BACKWARDCAM;
				break;
			default:
				break;
		}
	}
	
	public void ShakeCamFor(float dur, float ampGain = 0.5f, float frqGain = 1f)
	{
		StartCoroutine(DelShakeCam(dur,ampGain, frqGain));
	}
	
	IEnumerator DelShakeCam(float dur, float ampGain = 0.5f, float frqGain = 1f)
	{
		ShakeCam(ampGain, frqGain);
		yield return new WaitForSeconds(dur);
		UnShakeCam(ampGain, frqGain);
	}



	public void ShakeCam(float ampGain, float frqGain)
	{
		//switch (curCamStat)
		//{
		//	case CamStatus.Freelook:
		//	case CamStatus.Locked:
		//		break;
		//	case CamStatus.Aim:
		//		camShaker.m_AmplitudeGain = ampGain;
		//		camShaker.m_FrequencyGain = frqGain;
		//		break;

		//	default:
		//		break;
		//}
		for (int i = 0; i < camShakers.Count; i++)
		{
			camShakers[i].m_AmplitudeGain += ampGain;
			camShakers[i].m_FrequencyGain += frqGain;
		}
	}

	
	public void UnShakeCam(float ampGain, float frqGain)
	{
		//switch (curCamStat)
		//{
		//	case CamStatus.Freelook:
		//	case CamStatus.Locked:
		//		break;
		//	case CamStatus.Aim:
		//		camShaker.m_AmplitudeGain = 0;
		//		camShaker.m_FrequencyGain = 0;
		//		break;

		//	default:
		//		break;
		//}
		for (int i = 0; i < camShakers.Count; i++)
		{
			camShakers[i].m_AmplitudeGain -= ampGain;
			camShakers[i].m_FrequencyGain -= frqGain;
		}
	}
	public void Blind(bool stat)
	{
		if(blinded != stat)
		{
			blinded = stat;
			v.gameObject.SetActive(stat);
		}
	}
}
