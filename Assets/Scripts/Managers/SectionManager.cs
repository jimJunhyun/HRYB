using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState{
	None = -1,
	InCave,
	OutCave,
	Fight,
}

public class SectionManager : MonoBehaviour
{
    public GameState curState = GameState.InCave;
	GameState prevState = GameState.InCave;

	public GameObject sections;

	public List<StageProceedVolume> allSections; 

	private void Awake()
	{
		sections = GameObject.Find("StageSections");
		sections.GetComponentsInChildren<StageProceedVolume>(allSections);
	}

	public void ProceedTo(GameState state)
	{
		if(curState != state)
		{
			prevState = curState;
			curState = state;
			GameManager.instance.audioPlayer.PlayBgm($"{state}Bgm");
			if (state == GameState.OutCave)
			{
				Debug.Log("TIMELINED");
				GameManager.instance.timeliner2.Play();
				StartCoroutine(GameManager.instance.DelInputCtrl((float)GameManager.instance.timeliner2.duration));
				GameManager.instance.qManager.NextIf(Quests.ESCAPECAVE);
			}
		}
		
	}

	public void Proceed()
	{
		prevState = curState;
		curState += 1;
	}

	public void RevertState()
	{
		GameState stat = prevState;
		prevState = curState;
		curState = stat;
	}
}
