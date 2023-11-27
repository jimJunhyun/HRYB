using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleBtn : MonoBehaviour
{
	private Image img;
	public Sprite BtnUI;
	public Sprite OnBtnUI;


	private void Start()
	{
		img = GetComponent<Image>();
	}

	public void Enter()
	{
		img.sprite = OnBtnUI;
	}

	public void Exit()
	{
		img.sprite = BtnUI;
	}

	public void GameStart()
	{
		SceneManager.LoadScene("DemoGame");
		Debug.Log("시작");
	}
	public void GameExit()
	{
		Application.Quit();
		Debug.Log("끝");

	}
}
