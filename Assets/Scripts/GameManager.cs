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
	public PlayerInter pinter;
	public PlayerMove pMove;
	public PlayerAttack pAtk;
	public PlayerLife pLife;
	public PlayerCast pCast;
	public CinemachineFreeLook pCam;
	public CinemachineVirtualCamera aimCam;
	public CraftManager craftManager;

	public Arrow arrow;

	public float ampGain = 0.5f;
	public float frqGain = 1f;

	public StatusEffects statEff;

	CinemachineBasicMultiChannelPerlin aimCamShaker;

	public bool lockMouse;

	public CamStatus curCamStat;

	private void Awake()
	{
		instance = this;

		LockCursor();

		player = GameObject.Find("Player");
		pinp = player.GetComponent<PlayerInput>();
		pinven = player.GetComponent<PlayerInven>();
		pAtk = player.GetComponent<PlayerAttack>();
		pMove = player.GetComponent<PlayerMove>();
		pLife = player.GetComponent<PlayerLife>();
		pCast = player.GetComponent<PlayerCast>();
		pinter = player.GetComponent<PlayerInter>();
		pCam = GameObject.Find("PCam").GetComponent<CinemachineFreeLook>();
		itemManager = GameObject.Find("ItemManager").GetComponent<ItemManager>();
		aimCam = GameObject.Find("AimCam").GetComponent<CinemachineVirtualCamera>();
		aimCamShaker = aimCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
		craftManager = GameObject.Find("CraftManager").GetComponent<CraftManager>();
		statEff = new StatusEffects();
		SwitchTo(CamStatus.Freelook);
	}

	public void LockCursor()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	public void UnLockCursor()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	public void LockUnlockCursor()
	{
		if (lockMouse)
		{
			lockMouse = false;
			UnLockCursor();
		}
		else
		{
			lockMouse = true;
			LockCursor();
		}
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

	public void CalcCamVFov(float differ)
	{
		pCam.m_Lens.FieldOfView += differ;
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

	public static float ClampAngle(float angle, float min, float max)
	{
		float start = (min + max) * 0.5f - 180;
		float floor = Mathf.FloorToInt((angle - start) / 360) * 360;
		return Mathf.Clamp(angle, min + floor, max + floor);
	}
}
