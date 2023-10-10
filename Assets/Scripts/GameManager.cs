using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public enum CamStatus
{
	Freelook,
	Locked,
	Aim,

}

public class GameManager : MonoBehaviour
{
	public const int FORWARDCAM = 20;
	public const int BACKWARDCAM = 10;

    public static GameManager instance;

    public GameObject player;
	public PlayerInput pinp;
	public ItemManager itemManager;
	public PlayerInven pinven;
	public PlayerMove pMove;
	public PlayerAttack pAtk;
	public CinemachineFreeLook pCam;
	public CinemachineVirtualCamera aimCam;

	public Arrow arrow;

	public float ampGain = 0.5f;
	public float frqGain = 1f;

	CinemachineBasicMultiChannelPerlin aimCamShaker;

	public bool lockMouse;

	CamStatus curCamStat;

	private void Awake()
	{
		instance = this;

		if (lockMouse)
		{
			Cursor.lockState = CursorLockMode.Locked;
			//Cursor.visible = false;
		}

		player = GameObject.Find("Player");
		pinp = player.GetComponent<PlayerInput>();
		pinven = player.GetComponent<PlayerInven>();
		pAtk = player.GetComponent<PlayerAttack>();
		pMove = player.GetComponent<PlayerMove>();
		pCam = GameObject.Find("PCam").GetComponent<CinemachineFreeLook>();
		itemManager = GameObject.Find("ItemManager").GetComponent<ItemManager>();
		aimCam = GameObject.Find("AimCam").GetComponent<CinemachineVirtualCamera>();
		aimCamShaker = aimCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

		SwitchTo(CamStatus.Freelook);
	}

	public void ShakeCam()
	{
		switch (curCamStat)
		{
			case CamStatus.Freelook:
			case CamStatus.Locked:
				break;
			case CamStatus.Aim:
				aimCamShaker.m_AmplitudeGain = ampGain;
				aimCamShaker.m_FrequencyGain = frqGain;
				break;
			
			default:
				break;
		}
	}

	public void UnShakeCam()
	{
		switch (curCamStat)
		{
			case CamStatus.Freelook:
			case CamStatus.Locked:
				break;
			case CamStatus.Aim:
				aimCamShaker.m_AmplitudeGain = 0;
				aimCamShaker.m_FrequencyGain = 0;
				break;

			default:
				break;
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
}
