using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TitleBtn : MonoBehaviour
{
	private Image img;
	public Sprite BtnUI;
	public Sprite OnBtnUI;
	public GameObject optionUI;


	private TMP_Text text;

	public bool isTarget;

	private void Awake()
	{
		isTarget = false;
		img = GetComponent<Image>();
		text = GetComponentInChildren<TMP_Text>();
	}

	private void Start()
	{
		img.sprite = BtnUI;
		text.color = Color.white;
		if(optionUI)
			optionUI.gameObject.SetActive(false);
		Cursor.visible = true;

		Cursor.lockState = CursorLockMode.None;
	}

	public void OnOption()
	{
		if (optionUI)
			optionUI.gameObject.SetActive(true);
	}

	public void OffOption()
	{
		if (optionUI)
			optionUI.gameObject.SetActive(false);
	}

	public void OffOption2()
	{
		GameManager.instance.uiManager.OnOffOption();
	}

	public void Enter()
	{
		img.sprite = OnBtnUI;
		text.color = Color.black;
	}

	public void Exit()
	{
		img.sprite = BtnUI;
		text.color = Color.white;
	}

	public void GameStart()
	{
		SceneManager.LoadScene("Loading");
		Debug.Log("시작");
	}
	public void GameExit()
	{
		Application.Quit();
		Debug.Log("끝");

	}
	
	private void Update()
	{
		if(isTarget) 
		{
			img.sprite = OnBtnUI;
			text.color = Color.black;
		}
	}
}
