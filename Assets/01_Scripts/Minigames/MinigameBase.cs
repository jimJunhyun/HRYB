using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MinigameBase : MonoBehaviour
{
	//public List<string> feedbackerName;
	const float ENDDELSEC = 1.5f;
	public string minigameSceneName;

	public Minigames myMode;

	protected ItemAmountPair minigameTarget;

	GameObject minigameZone;

	protected List<Animator> feedbacks;
	protected bool gameStarted = false;
	
	private readonly int ActHash = Animator.StringToHash("Act");

	Image resBgnd;
	TextMeshProUGUI resTxt;
	
	public virtual void Awake()
	{
		if(minigameSceneName == ""){
			minigameSceneName = MinigameManager.GetSceneName(myMode);
		}
		minigameZone = GameObject.Find(minigameSceneName);
		feedbacks = new List<Animator>(GetComponentsInChildren<Animator>());
		gameStarted = false;

		resBgnd = GameObject.Find("MinigameResBgnd").GetComponent<Image>();
		resTxt = GameObject.Find("MinigameResTxt").GetComponent<TextMeshProUGUI>();

		resBgnd.enabled = false;
		resTxt.enabled = false;
	}

	public virtual void StartGame(ItemAmountPair objName)
	{
		minigameTarget = objName;
		resBgnd.enabled = false;
		resTxt.enabled = false;
		Debug.Log($"아이템 : {minigameTarget.info.MyName}에 대한 미니게임 시작.");
		minigameZone.SetActive(true);
	}

	public virtual void EndGame()
	{
		Debug.Log($"아이템 : {minigameTarget.info.MyName}에 대한 미니게임 성공.");
		resBgnd.enabled = true;
		resTxt.enabled = true;
		resTxt.text = $"<#00dd00>{minigameTarget.info.MyName}</color> : 가공하는 데에 <#00dd00>성공</color>했습니다.";

		GameManager.instance.StartCoroutine(DelEndGame());
	}

	public virtual void FailGame()
	{

		Debug.Log($"아이템 : {minigameTarget.info.MyName}에 대한 미니게임 실패.");
		resBgnd.enabled = true;
		resTxt.enabled = true;
		resTxt.text = $"<#00dd00>{minigameTarget.info.MyName}</color> : 가공하는 데에 <#dd0000>실패</color>했습니다.";


		GameManager.instance.StartCoroutine(DelEndGame());
	}

	public virtual bool DoGameCheck()
	{
		throw new UnityException($"{myMode} 미니게임의 성공 조건이 없습니다!");
		return false;
	}

	public virtual void ShowFeedback()
	{
		for (int i = 0; i < feedbacks.Count; i++)
		{
			feedbacks[i].SetTrigger(ActHash);
		}
	}

	public virtual void PerformGame()
	{

		gameStarted = true;
	}

	IEnumerator DelEndGame()
	{
		yield return new WaitForSecondsRealtime(ENDDELSEC);
		minigameZone.SetActive(false);

		MinigameManager.UnloadMinigame();
	}
}
