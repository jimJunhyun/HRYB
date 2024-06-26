using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stirrer : MinigameBase
{
	public float targetRad;
	public float targetAppearRad;

	public float moveBallRad;

	public float ballSpeed;

	public int successThreshold;

    float playZoneRad;
	
	int successCount;


	Vector2 ballMoveDir;
	Vector2 posDiff;

	RectTransform playZone;
	RectTransform target;
	Transform ball;

	private readonly int DirectionHash = Animator.StringToHash("Direction");

	public override void Awake()
	{
		base.Awake();

		ball = transform.Find("StirCanvas/StirBall");
		target = transform.Find("StirCanvas/Target").GetComponent<RectTransform>();
		playZone = transform.Find("StirCanvas/PlayZone").GetComponent<RectTransform>();
		
		playZoneRad	= playZone.sizeDelta.x / 2;

		SetTarget();
	}

	private void Update()
	{
		if (gameStarted)
		{
			if (Input.GetMouseButtonDown(0))
			{
				posDiff = (ball.position - Input.mousePosition);
				if (posDiff.sqrMagnitude <= moveBallRad * moveBallRad)
				{
					ballMoveDir = posDiff.normalized;
					ShowFeedback();
				}

				if (posDiff.x > 0)
				{
					for (int i = 0; i < feedbacks.Count; i++)
					{
						feedbacks[i].SetBool(DirectionHash, true);
					}
				}
				else
				{
					for (int i = 0; i < feedbacks.Count; i++)
					{
						feedbacks[i].SetBool(DirectionHash, false);
					}
				}
			}


			ball.Translate(ballMoveDir * ballSpeed * Time.unscaledDeltaTime);

			if (FailCheck())
			{
				FailGame();
			}

			if (CircleCheck())
			{
				successCount += 1;
				SetTarget();
				if (DoGameCheck())
				{
					EndGame();
				}
			}
		}
		
	}

	public override void StartGame(ItemAmountPair objName)
	{
		base.StartGame(objName);
		
		
	}

	public override void EndGame()
	{
		PreProcess.ApplyProcessTo(ProcessType.Stir, minigameTarget);
		Debug.Log("데침");
		base.EndGame();
	}

	void SetTarget()
	{
		target.sizeDelta = Vector2.one * targetRad;
		target.localPosition = Random.insideUnitCircle * targetAppearRad;
	}

	bool FailCheck()
	{
		return (ball.position - playZone.position).sqrMagnitude > playZoneRad * playZoneRad;
	}
	
	bool CircleCheck()
	{
		return (ball.position - target.position).sqrMagnitude <= targetRad * targetRad;
	}

	public override bool DoGameCheck()
	{
		return successCount >= successThreshold;
	}
}
