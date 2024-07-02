using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>
/// Default = 기본, Free = 자유시점
/// </summary>
public enum CameraMode
{
	Default = 0,
	Free = 1,
}
/// <summary>
/// Default = 자유시점, Fixed = 자유시점 고정
/// </summary>
public enum FreeCameraMode
{
	Default = 0,
	Fixed = 1
}
public class FreeCameraSwitcher : MonoBehaviour
{
	private Camera mainCam;
	private Camera freeCam;
	private Canvas canvas;
	public CameraMode cameraMode = CameraMode.Default;
	public FreeCameraMode freeCameraMode = FreeCameraMode.Default;
	private KeyCode switchCameraModeKey = KeyCode.F8;
	private KeyCode switchFreeCameraModeKey = KeyCode.F9;
	private KeyCode uiToggleModeKey;

	private PlayerInput input;

	int tempPt = 0;

	private void Awake()
	{
		uiToggleModeKey = KeyCode.F10;
#if UNITY_EDITOR
		uiToggleModeKey = KeyCode.F7;
#endif
		mainCam = Camera.main;

		if(freeCam == null)
		{
			freeCam = Instantiate(mainCam, this.transform);
			freeCam.gameObject.SetActive(false);
			freeCam.AddComponent<FreeCamera>();
			Destroy(freeCam.GetComponent<AudioListener>());
			Destroy(freeCam.GetComponent<CinemachineBrain>());
		}

		if(GameManager.instance != null)
		{
			input = GameManager.instance.pinp;
		}
		else
		{
			input = GameObject.Find("Player").GetComponent<PlayerInput>();
		}

		canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
	}
	

    void Update()
    {
		if(Input.GetKeyDown(uiToggleModeKey))
		{
			if (!canvas.enabled)
				canvas.enabled = true;
			else
				canvas.enabled = false;
		}

		if (Input.GetKeyDown(switchCameraModeKey))
		{
			int idx = (int)cameraMode;
			if (idx + 1 > 1)
			{
				idx = 0;
			}
			else idx++;

			cameraMode = (CameraMode)idx;

			switch (cameraMode)
			{
				case CameraMode.Default:
					SwitchCamera(false);
					break;
				case CameraMode.Free:
					SwitchCamera(true);

					break;
			}
		}
		if(cameraMode == CameraMode.Free)
		{
			if (Input.GetKeyDown(switchFreeCameraModeKey))
			{
				int idx = (int)freeCameraMode;
				if (idx + 1 > 1)
				{
					idx = 0;
				}
				else idx++;

				freeCameraMode = (FreeCameraMode)idx;

				switch (freeCameraMode)
				{
					case FreeCameraMode.Default:
						freeCam.GetComponent<FreeCamera>().enabled = true;
						input.SwitchCurrentActionMap("FreeCamera");

						break;
					case FreeCameraMode.Fixed:
						freeCam.GetComponent<FreeCamera>().enabled = false;
						input.SwitchCurrentActionMap("Player");
						break;
				}
				print($"Action Map = {GameManager.instance.pinp.currentActionMap}");
			}

			if (Input.GetKeyDown(KeyCode.Tab))
			{
				tempPt = ((tempPt + 1) % (GameManager.instance.pActor.life as PlayerLife).spawnPoint.Length);
				freeCam.transform.position = (GameManager.instance.pActor.life as PlayerLife).spawnPoint[tempPt];
				freeCam.transform.position += Vector3.up * 3;
				freeCam.transform.LookAt((GameManager.instance.pActor.life as PlayerLife).spawnPoint[tempPt]);
			}

		}

	}
	
	private void SwitchCamera(bool isFree)
	{
		if(isFree)
		{
			freeCam.gameObject.SetActive(true);
			freeCam.transform.position = mainCam.transform.position;
			freeCam.transform.rotation = mainCam.transform.rotation;

			input.SwitchCurrentActionMap("FreeCamera");
			freeCam.GetComponent<FreeCamera>().enabled = true;
			freeCameraMode = FreeCameraMode.Default;
		}
		else
		{
			freeCam.gameObject.SetActive(false);

			input.SwitchCurrentActionMap("Player");
			freeCam.GetComponent<FreeCamera>().enabled = false;
		}
	}
}