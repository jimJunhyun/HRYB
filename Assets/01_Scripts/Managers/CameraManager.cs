using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class CameraManager : MonoBehaviour
{
	public List<CinemachineBasicMultiChannelPerlin> camShakers = new List<CinemachineBasicMultiChannelPerlin>();
	SkillProduction _skillProduct;
	public CamStatus curCamStat;
	GameObject _middleCamObj = null;
	
	public const int FORWARDCAM = 20;
	public const int BACKWARDCAM = 10;
	
	bool blinded = false;
	Volume v;
	Camera _main;

	Coroutine ongoing;
	
	public CinemachineFreeLook _pCam;
	public CinemachineFreeLook pCam
	{
		get
		{
			if(_pCam == null)
			{
				_pCam = GetComponent<CinemachineFreeLook>();
			}
			return _pCam;
		}
	}
	public CinemachineVirtualCamera aimCam;

	public float minZoom;
	public float maxZoom;
	public float zoomSpd;

	float originYSpeed;
	float originFOV;

	public static CameraManager instance;

	PlayerMove _playerModule;


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

	public void Wheel(InputAction.CallbackContext context)
	{
		Vector2 scr = context.ReadValue<Vector2>();
		if (scr.y == 0)
			return;

		for (int i = 0; i < 3; i++)
		{
			_pCam.m_Orbits[i].m_Radius -= scr.y * Time.deltaTime * zoomSpd;
			_pCam.m_Orbits[i].m_Radius = Mathf.Clamp(_pCam.m_Orbits[i].m_Radius, minZoom, maxZoom);
		}
	}
	

	public void RegisterSkillCam(SkillProduction _sk)
	{
		if(_skillProduct != null)
			_skillProduct.End();

		_skillProduct = _sk;
	}


	private void Awake()
	{
		instance = this;
		_main = Camera.main;
		

		aimCam = GameObject.Find("AimCam").GetComponent<CinemachineVirtualCamera>();



		for (int i = 0; i < 3; i++)
		{
			camShakers.Add(pCam.GetRig(i).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>());
		}

		v = _main.GetComponentInChildren<Volume>();
		//Debug.LogError(camShakers.Count);
		for (int i = 0; i < camShakers.Count; i++)
		{
			camShakers[i].m_AmplitudeGain = 0;
			camShakers[i].m_FrequencyGain = 0;
		}

		originYSpeed = pCam.m_YAxis.m_MaxSpeed;
		originFOV = pCam.m_Lens.FieldOfView;

		if (_middleCamObj == null)
			_middleCamObj = new GameObject();
	}

	private void Start()
	{
		_playerModule = (GameManager.instance.pActor.move as PlayerMove);
		SwitchTo(CamStatus.Freelook);
	}

	private void Update()
	{
		if(_playerModule.moveModuleStat.Paused == false && _skillProduct != null)
		{
			_skillProduct.End();
			_skillProduct = null;
		}
		
		if(curCamStat == CamStatus.Locked && _playerModule.GetActor().atk.target != null)
		{
			Vector3 a = _playerModule.transform.position;
			Vector3 b = _playerModule.GetActor().atk.target.position;
			float value = ((b - a).sqrMagnitude) / (_playerModule.lockOnDist * _playerModule.lockOnDist);
			//Debug.LogError($"Value : {value}");

			pCam.m_XAxis.Value = Mathf.Clamp(pCam.m_XAxis.Value, -37f, 37f);
			pCam.m_YAxis.Value = 0.48f;
			Vector3 vec;

			if (value >= 0.25f)
				vec = Vector3.Lerp(a, b + new Vector3(0, 2.4f, 0), value);
			else
				vec = Vector3.Lerp(b + new Vector3(0, 1.2f, 0), a, value);


			_middleCamObj.transform.position = Vector3.Lerp(_middleCamObj.transform.position, vec, Time.deltaTime * 5);
			//pCam.m_YAxis.Value
		}
		else
		{
			_middleCamObj.transform.position = _playerModule.transform.position;
		}

		if(_playerModule.GetActor().atk.target==null)
		{
			SwitchTo(CamStatus.Freelook);
		}

	}

	public void SwitchTo(CamStatus stat)
	{
		curCamStat = stat;
		switch (stat)
		{
			case CamStatus.Freelook:
				pCam.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
				pCam.Priority = FORWARDCAM;
				pCam.LookAt = _playerModule.transform.Find("Middle").transform;
				aimCam.Priority = BACKWARDCAM;
				break;
			case CamStatus.Aim:
				aimCam.Priority = FORWARDCAM;
				pCam.Priority = BACKWARDCAM;
				break;
			case CamStatus.Locked:
				pCam.m_BindingMode = CinemachineTransposer.BindingMode.LockToTargetWithWorldUp;

				pCam.Priority = FORWARDCAM;
				aimCam.Priority = BACKWARDCAM;
				
				pCam.LookAt = _middleCamObj.transform;
				

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

	public void FreezeCamX(bool useCurrentCamX = false)
	{
		if (useCurrentCamX)
		{
			pCam.m_XAxis.m_MinValue = pCam.m_XAxis.Value;
			pCam.m_XAxis.m_MaxValue = pCam.m_XAxis.Value;
		}
		pCam.m_XAxis.m_Wrap = false;
	}

	public void UnfreezeCamX()
	{
		if(!pCam)
		{
			return;
		}
		pCam.m_XAxis.m_MinValue = 0;
		pCam.m_XAxis.m_MaxValue = 0;
		pCam.m_XAxis.m_Wrap = true;
	}

	public void FreezeCamY()
	{
		pCam.m_YAxis.m_MaxSpeed = 0;
		//Debug.Log("Y가정지됨ㅋㅋㅋㅋ");
	}

	public void UnfreezeCamY()
	{
		pCam.m_YAxis.m_MaxSpeed = originYSpeed;
		//Debug.Log("Y가정지안됨ㅋㅋㅋㅋ");
	}

	public void Zoom(float power, float lerpSec = 0.75f)
	{
		ongoing = StartCoroutine(DelZoom(power, true, lerpSec));
	}

	IEnumerator DelZoom(float pow, bool zooming, float lerpSec)
	{
		float t = 0;
		float v = pCam.m_Lens.FieldOfView;
		while (t< lerpSec)
		{
			yield return null;
			t += Time.deltaTime;
			if (zooming)
			{
				pCam.m_Lens.FieldOfView = Mathf.Lerp(v, v - pow, t / lerpSec);
			}
			else
			{
				pCam.m_Lens.FieldOfView = Mathf.Lerp(v, originFOV, t / lerpSec);

			}
		}
	}

	public void RevertZoom(float lerpSec = 0.75f)
	{
		if (ongoing!= null)
		{
			StopCoroutine(ongoing);
		}
		StartCoroutine(DelZoom(0, false, lerpSec));
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

		if (_skillProduct != null)
		{
			_skillProduct._shakes.m_AmplitudeGain = ampGain;
			_skillProduct._shakes.m_FrequencyGain = frqGain;

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
