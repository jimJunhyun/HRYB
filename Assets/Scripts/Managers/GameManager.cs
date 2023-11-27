using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.Playables;

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
	public SectionManager sManager;
	public PrefabManager pManager;

	public PlayableDirector timeliner;
	public PlayableDirector timeliner2;

	public ImageManager imageManager;

	public Arrow arrow;

	public AudioPlayer audioPlayer;

	public Terrain terrain;

	[Header("따로 설정이 필요함")]
	public Sprite uiBase;
	public TMPro.TMP_FontAsset tmpText;

	public float ampGain = 0.5f;
	public float frqGain = 1f;

	public StatusEffects statEff;

	CinemachineBasicMultiChannelPerlin aimCamShaker;

	public bool lockMouse;

	public Transform outCaveTmp;

	public CamStatus curCamStat;

	public WaitForSeconds waitSec = new WaitForSeconds(1.0f);

	public float curVFov = CAMVFOV;
	public float? fixedCamVFov = null;

	private void Awake()
	{
		instance = this;

		LockCursor();

		qManager = new QuestManager(
			"사슴 시체의 냄새가 나.\n 약재를 얻어볼까?",
			"연못 주위에 있는 덤불을 채집하고\n 밧줄을 제작하자.",
			"먼저 녹각을 손질해서 녹용을 만들어보자.",
			"녹용 2개와 녹제 1개로 도약탕을 만들 수 있어. 물도 넣는걸 잊지마.",
			"약을 먹고 동굴 밖으로 빠져나가자.",
			"쓸모있는 나뭇가지가 보이네.\n 활을 제작해보자.",
			"방금 도망간 곰을 쫓아가 사냥해볼까?"
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
		terrain = GameObject.Find("Terrain").GetComponentInChildren<Terrain>();
		audioPlayer = GameObject.Find("AudioManager").GetComponent<AudioPlayer>();
		sManager = GameObject.Find("SectionManager").GetComponent<SectionManager>();
		timeliner = GameObject.Find("Timeliner").GetComponent<PlayableDirector>(); //////////#####타임라인매니저
		timeliner2 = GameObject.Find("Timeliner2").GetComponent<PlayableDirector>();
		pManager = GameObject.Find("PrefabManager").GetComponent<PrefabManager>();
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

	public void TPToOutCave()
	{
		player.transform.position = outCaveTmp.position;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			pinven.ObtainWeapon();
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

	public IEnumerator DelInputCtrl(float sec)
	{
		pinp.DeactivateInput();
		Debug.Log("DEACT");
		yield return new WaitForSeconds(sec);
		Debug.Log("ACT");
		pinp.ActivateInput();
	}

	public static float[] GetTerrainData(Vector3 pos, Terrain t)
	{
		Vector3 tmp = t.transform.position;
		TerrainData tData = t.terrainData;

		int mapX = Mathf.RoundToInt((pos.x - tmp.x) / tData.size.x * tData.alphamapWidth) ;
		int mapZ = Mathf.RoundToInt((pos.z - tmp.z) / tData.size.z * tData.alphamapHeight) ;

		float[,,] sampleData = tData.GetAlphamaps(mapX, mapZ, 1, 1);
		float[] infos = new float[sampleData.GetUpperBound(2) + 1];
		for (int i = 0; i < infos.Length; i++)
		{
			infos[i] = sampleData[0, 0, i];
		}
		return infos;
	}

	public static string GetLayerName(Vector3 pos, Terrain t)
	{
		float[] info = GetTerrainData(pos, t);
		float largest = float.MinValue;
		int largestIdx = -1;
		for (int i = 0; i < info.Length; i++)
		{
			if(info[i] > largest)
			{
				largest = info[i];
				largestIdx = i;
			}
		}

		return t.terrainData.terrainLayers[largestIdx].name;
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		float start = (min + max) * 0.5f - 180;
		float floor = Mathf.FloorToInt((angle - start) / 360) * 360;
		return Mathf.Clamp(angle, min + floor, max + floor);
	}
}
