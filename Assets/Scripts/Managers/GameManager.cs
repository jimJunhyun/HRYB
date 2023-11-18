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

	public const int PLAYERLAYER = 7;
	public const int INTERABLELAYER = 8;
	public const int GROUNDLAYER = 11;
	public const int CLIMBABLELAYER = 17;
	public const int HOOKABLELAYER = 19;

	public const float CAMVFOV = 55;

	public static GameManager instance;

    public GameObject player;
	public PlayerInput pinp;
	public PlayerInven pinven;
	public Actor pActor;
	public CinemachineFreeLook pCam;
	public CinemachineVirtualCamera aimCam;
	public CraftManager craftManager;
	public UIManager uiManager;
	public QuestManager qManager;

	public ImageManager imageManager;

	public Arrow arrow;

	[Header("따로 설정이 필요함")]
	public Sprite uiBase;
	public TMPro.TMP_FontAsset tmpText;

	public float ampGain = 0.5f;
	public float frqGain = 1f;

	public StatusEffects statEff;

	CinemachineBasicMultiChannelPerlin aimCamShaker;

	public bool lockMouse;

	public CamStatus curCamStat;

	public WaitForSeconds waitSec = new WaitForSeconds(1.0f);

	public float curVFov = CAMVFOV;
	public float? fixedCamVFov = null;

	private void Awake()
	{
		instance = this;

		LockCursor();

		qManager = new QuestManager(
			"점프를 하시오",
			"밧줄을 거시오",
			"곰을 잡으시오",
			"수고하시었소"
			);

		player = GameObject.Find("Player");
		pinp = player.GetComponent<PlayerInput>();
		pinven = player.GetComponent<PlayerInven>();
		pActor = player.GetComponent<Actor>();
		pCam = GameObject.Find("PCam").GetComponent<CinemachineFreeLook>();
		aimCam = GameObject.Find("AimCam").GetComponent<CinemachineVirtualCamera>();
		aimCamShaker = aimCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
		craftManager = GameObject.Find("CraftManager").GetComponent<CraftManager>();
		imageManager = GameObject.Find("ImageManager").GetComponent<ImageManager>();
		uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
		statEff = new StatusEffects();
		SwitchTo(CamStatus.Freelook);

		
	}

	private void Start()
	{
		qManager.NextQuest();
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


	public void SetCamVFov(float val)
	{
		if(fixedCamVFov == null)
		{
			curVFov = val;
			pCam.m_Lens.FieldOfView = curVFov;
		}
		else
		{
			pCam.m_Lens.FieldOfView = (float)fixedCamVFov;
		}
	}

	public void SetFixedCamFov(float val)
	{
		fixedCamVFov = val;
	}

	public void ResetFixedCamFov()
	{
		fixedCamVFov = null;
		pCam.m_Lens.FieldOfView = curVFov;
	}

	public void CraftWithUI()
	{
		craftManager.crafter.CraftWith( uiManager.crafterUI.curRecipe);
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
