using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
	TMP_Text Title;
	TMP_Text SubTitle;
	TMP_Text Contents;

	public Choist choisePrefab;
	public GameObject choiseList;

	public int chosen = -1;

	public Dialogue currentShown;

	public Character talker;

	List<Choist> choists = new List<Choist>();

	internal bool stat;
	bool choiceStat;

	private void Awake()
	{
		Title = transform.GetChild(0).GetComponent<TMP_Text>();
		SubTitle = transform.GetChild(1).GetComponent<TMP_Text>();
		Contents = transform.GetChild(2).GetComponent<TMP_Text>();
	}

	private void Start()
	{
		Off();
	}


	//private void Update()
	//{
	//	if(Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.F))
	//	{
	//		currentShown?.OnClick();
	//	}
	//}

	public void ShowText(string text)
	{
		if (!stat)
		{
			this.gameObject.SetActive(true);
			stat = true;
			GameManager.instance.UnLockCursor();
			GameManager.instance.DisableCtrl(ControlModuleMode.Timeline);
			GameManager.instance.camManager.FreezeCamX(true);
			GameManager.instance.camManager.FreezeCamY();
			GameManager.instance.camManager.Zoom(30);

			GameManager.instance.uiManager.basicUIGroup.SetActive(false);
		}

		if(talker == null)
		{
			Title.text = "나";
		}
		else
		{
			Title.text = talker.baseName;
			SubTitle.text = talker.subName;
		}

		Contents.text = text;
	}

	public void Off()
	{
		this.gameObject.SetActive(false);
		choiseList.SetActive(false);
		currentShown = null;
		talker = null;
		chosen = -1;
		stat = false;
		choiceStat = false;
		GameManager.instance.LockCursor();
		GameManager.instance.EnableCtrl(ControlModuleMode.Timeline);
		GameManager.instance.camManager.UnfreezeCamX();
		GameManager.instance.camManager.UnfreezeCamY();
		GameManager.instance.camManager.RevertZoom();
		GameManager.instance.uiManager.basicUIGroup.SetActive(true);
		
		
	}

	public void ShowChoice(List<string> choice)
	{
		if (!choiceStat)
		{
			choiseList.SetActive(true);
		}
		for(int i = choice.Count - 1; i >= 0; i--) 
		{
			Choist cc = Instantiate(choisePrefab, choiseList.transform);
			cc.SetText(choice[i]);
			cc.choiceNum = i;
			choists.Add(cc);
		}
	}

	public void OffChoice()
	{
		choiceStat = false;
		for (int i = 0; i < choists.Count; i++)
		{
			Destroy(choists[i].gameObject);
		}
		choists.Clear();
		
		choiseList.SetActive(false);
	}

}
