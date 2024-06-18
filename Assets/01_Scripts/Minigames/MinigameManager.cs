using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Minigames
{
	None,
	Cutting,
	Frying,
	Stirring,
	Mashing,

}
public class MinigameManager
{
	public const string MINIGAMENAME = "Minigame";

	static Minigames curMinigame = Minigames.None;

    public static void LoadMinigame(Minigames mode, ItemAmountPair itemInfo)
	{
		if (curMinigame == Minigames.None)
		{
			GameManager.instance.StartCoroutine(DoLoadMinigame(GetSceneName(mode), itemInfo));

			Time.timeScale = 0;
			GameManager.instance.pinp.DeactivateInput();
			curMinigame = mode;
		}
	}

	static IEnumerator DoLoadMinigame(string sceneName, ItemAmountPair itemInfo)
	{
		AsyncOperation oper = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

		GameManager.instance.uiManager.OffCanvas();

		while (!oper.isDone)
		{
			yield return null;
		}

		GameObject.Find(sceneName).GetComponent<MinigameBase>().StartGame(itemInfo);
	}

	public static void UnloadMinigame()
	{
		if (curMinigame != Minigames.None)
		{
			GameManager.instance.StartCoroutine(DoUnloadMinigame());
		}
	}

	static IEnumerator DoUnloadMinigame()
	{
		AsyncOperation oper = SceneManager.UnloadSceneAsync(GetSceneName(curMinigame));

		while (!oper.isDone)
		{
			yield return null;
		}

		GameManager.instance.uiManager.OnCanvas();
		//Time.timeScale = 1;
		GameManager.instance.pinp.ActivateInput();
		curMinigame = Minigames.None;
	}

	public static string GetSceneName(Minigames mode)
	{
		switch (mode)
		{
			case Minigames.Cutting:
				return $"Cut{MINIGAMENAME}";
			case Minigames.Frying:
				return $"Fry{MINIGAMENAME}";
			case Minigames.Stirring:
				return $"Stir{MINIGAMENAME}";
			case Minigames.Mashing:
				return $"Mash{MINIGAMENAME}";
			default:
				throw new UnityException($"{mode} : 해당 모드는 존재하지 않음.");
		}
	}
}
