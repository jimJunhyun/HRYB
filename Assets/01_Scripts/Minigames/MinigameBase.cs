using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameBase : MonoBehaviour
{
	//public List<string> feedbackerName;
	public string minigameSceneName;

	public Minigames myMode;

	protected ItemAmountPair minigameTarget;

	GameObject minigameZone;

	protected List<Animator> feedbacks;
	protected bool gameStarted = false;
	
	private readonly int ActHash = Animator.StringToHash("Act");

	
	public virtual void Awake()
	{
		if(minigameSceneName == ""){
			minigameSceneName = MinigameManager.GetSceneName(myMode);
		}
		minigameZone = GameObject.Find(minigameSceneName);
		feedbacks = new List<Animator>(GetComponentsInChildren<Animator>());
		gameStarted = false;
	}

	public virtual void StartGame(ItemAmountPair objName)
	{
		minigameTarget = objName;
		Debug.Log($"아이템 : {minigameTarget.info.MyName}에 대한 미니게임 시작.");
		minigameZone.SetActive(true);
	}

	public virtual void EndGame()
	{
		Debug.Log($"아이템 : {minigameTarget.info.MyName}에 대한 미니게임 성공.");
		minigameZone.SetActive(false);

		MinigameManager.UnloadMinigame();
	}

	public virtual void FailGame()
	{

		Debug.Log($"아이템 : {minigameTarget.info.MyName}에 대한 미니게임 실패.");
		minigameZone.SetActive(false);

		MinigameManager.UnloadMinigame();
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
}
