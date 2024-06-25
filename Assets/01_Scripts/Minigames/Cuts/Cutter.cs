using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Cutter : MinigameBase
{ 
	public float errorDistance;
	public float clickGap;

	public float textFloatTime;

	public int needSuccessCount;
	public int maxTryCount;

	public Vector3 txtOffset;

	
	int tries;
	int curSuccessCount;

	float prevClick;

	Transform targ;
	Transform cursor;

	BarSwiftMove scroller;

	TextMeshProUGUI sucsText;

	Coroutine ongoings;


	public override void Awake()
	{
		targ = GameObject.Find("TargetPointForCuttingMinigame").transform;
		cursor = GameObject.Find("CursorForCuttingMinigame").transform;
		sucsText = GameObject.Find("SuccessStateForCuttingMinigame").GetComponent<TextMeshProUGUI>();
		scroller = GetComponentInChildren<BarSwiftMove>();
		base.Awake();
		sucsText.enabled = false;

		tries = 0;
		curSuccessCount = 0;
	}

	public bool DoBarCheck()
	{
		return (Vector3.Distance(targ.position, cursor.position) < errorDistance);
	}

	public override bool DoGameCheck() 
	{
		return curSuccessCount >= needSuccessCount;
	}

	public override void StartGame(ItemAmountPair objName)
	{
		if (Crafter.recipeItemTableTrim.ContainsKey(objName))
		{
			base.StartGame(objName);
		}
	}

	public override void EndGame()
	{
		if (GameManager.instance)
		{
			if (GameManager.instance.pinven.RemoveItem(minigameTarget.info))
			{
				GameManager.instance.craftManager.TrimWithName(minigameTarget.info.MyName);
			}
		}
		base.EndGame();
	}

	public override void PerformGame()
	{
		base.PerformGame();
		scroller.DoStart();
	}

	private void Update()
	{
		if (gameStarted)
		{

			if (Input.GetMouseButtonDown(0) && Time.unscaledTime - prevClick >= clickGap && ongoings == null)
			{
				prevClick = Time.unscaledTime;
				bool res = DoBarCheck();
				ongoings = StartCoroutine(ShowText(res));
					
				tries += 1;
				scroller.ChangeSpeed();
			}
		}
	}

	IEnumerator ShowText(bool state)
	{
		if (state)
		{
			sucsText.text = "성공!";
			curSuccessCount += 1;
			sucsText.color = Color.green;
			ShowFeedback();
		}
		else
		{
			sucsText.text = "실패...";
			sucsText.color = Color.red;
		}
		sucsText.transform.position = cursor.position + txtOffset;
		sucsText.enabled = true;
		float t = 0;
		while (t < textFloatTime)
		{
			yield return null;
			t += Time.unscaledDeltaTime;
			sucsText.alpha = 1 - t / textFloatTime;
		}
		sucsText.enabled = false;
		ongoings = null;

		if(tries >= maxTryCount)
		{
			if(DoGameCheck())
			{
				EndGame();
			}
			else
			{
				FailGame();
			}
		}
	}
}
