using UnityEngine;
using UnityEngine.UI;

public class Frier : MinigameBase
{
	
	public float windGap;
	public float windPower;

	public float windResetThreshold = 0.5f;

	public float timeLimit;

	public float fireDecreaseSpeed;
	public float fireAdequateAmt;
	[Tooltip("위와 아래 허용 범위")]
	public float adequateErrRange;

	public float gapDecPerLevel;
	public float minGap;

	public float WindGap
	{
		get => windGap - Mathf.Min(level * gapDecPerLevel, windGap - minGap);
	}

	public bool Windresets
	{
		get => Time.unscaledTime - prevIncreaseTime >= WindGap + windResetThreshold;
	}

	float fireAmt;

	float prevIncreaseTime;
	float accT = 0;

	int level;

	const float FIREMAXAMOUNT = 100;

	Slider fireBar;
	Slider fireMarker;
	Slider timerBar;
	RectTransform burnMarker;

	bool first = true;

	public override void Awake()
	{
		fireBar = GameObject.Find("FireBarForFryMinigame").GetComponent<Slider>();
		fireMarker = GameObject.Find("FireMarkerForFryMinigame").GetComponent<Slider>();
		timerBar = GameObject.Find("TimerBarForFryMinigame").GetComponent<Slider>();
		burnMarker = GameObject.Find("BurnMarkerForFryMinigame").GetComponent<RectTransform>();
		level = 0;
		base.Awake();


		fireBar.value = 0;
		timerBar.value = 0;
		accT = 0;
		//
		SetRandomTargets();
	}

	public override void StartGame(ItemAmountPair objName)
	{
		base.StartGame(objName);

		
	}

	private void Update()
	{
		if (gameStarted)
		{
			if (first)
			{
				first = false;
			}
			else
			{
				accT += Time.fixedUnscaledDeltaTime;
			}

			if (accT >= timeLimit)
			{
				if (DoGameCheck())
				{
					EndGame();
				}
				else
				{
					FailGame();
				}
			}
			else
			{
				timerBar.value = accT / timeLimit;
			}

			if (Input.GetMouseButtonDown(0))
			{
				if (Time.unscaledTime - prevIncreaseTime >= WindGap)
				{
					IncreaseFireBar();

					prevIncreaseTime = Time.unscaledTime;
				}
			}

			if (Windresets && level > 0)
			{
				level = 0;
			}

			DecreaseGauge();
		}
		
	}

	private void DecreaseGauge()
	{
		fireAmt -= fireDecreaseSpeed * Time.unscaledDeltaTime;
		fireAmt = Mathf.Clamp(fireAmt, 0, FIREMAXAMOUNT);

		//부채질게이지

		fireBar.value = fireAmt / FIREMAXAMOUNT;
	}

	public void SetRandomTargets()
	{
		SetTargets(Random.Range(FIREMAXAMOUNT * (adequateErrRange / 100.0f), FIREMAXAMOUNT * (85f - adequateErrRange) / 100.0f));
	}

	public void SetTargets(float amt, float adequater = -1)
	{
		fireAdequateAmt = amt;
		if(adequater > 0)
		{
			adequateErrRange = adequater;
		}
		SetMarker();
	}

	public void SetMarker()
	{
		fireMarker.value = 1 - fireAdequateAmt / FIREMAXAMOUNT;

		burnMarker.sizeDelta = Vector2.right * ((adequateErrRange * 2) / FIREMAXAMOUNT) * (fireBar.transform as RectTransform).sizeDelta.x;
	}

	public void IncreaseFireBar()
	{
		fireAmt += windPower;

		level += 1;

		ShowFeedback();
	}

	public override void EndGame()
	{
		if(fireAmt >= fireAdequateAmt + adequateErrRange)
		{
			PreProcess.ApplyProcessTo(ProcessType.Burn, minigameTarget);
			Debug.Log("태움");
		}
		else
		{
			PreProcess.ApplyProcessTo(ProcessType.Fry, minigameTarget);
			Debug.Log("볶음");
		}
		base.EndGame();
	}

	public override bool DoGameCheck()
	{
		return fireAmt >= fireAdequateAmt - adequateErrRange;
	}

	public override void ShowFeedback()
	{
		fireBar.value = fireAmt / FIREMAXAMOUNT;

		base.ShowFeedback();
	}

}	

