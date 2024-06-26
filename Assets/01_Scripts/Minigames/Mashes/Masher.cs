using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Masher : MinigameBase
{
	public float targetAngle;
	public float indicatorSpeed;

	public float timePerHit;
	public float totalTime;

	public int sucsThreshold;

	Image indicator;
	Image target;

	Slider timer;
	Slider hitTimer;

	float accT;
	float hitAcc;

	int spinMod = 1;
	int successCount;

	bool first = true;

	public override void Awake()
	{
		base.Awake();

		indicator= transform.Find("MashCanvas/PlayZone/Indicator").GetComponent<Image>();
		target= transform.Find("MashCanvas/PlayZone/Target").GetComponent<Image>();

		timer = transform.Find("MashCanvas/Timer").GetComponent<Slider>();
		hitTimer = transform.Find("MashCanvas/HitTimer").GetComponent<Slider>();
		accT = 0;
		hitAcc = 0;

		spinMod = 1;
		successCount = 0;

		SetTargetAngle(Random.Range(45, 75));
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
				accT += Time.unscaledDeltaTime;
				hitAcc += Time.unscaledDeltaTime;
				timer.value = accT / totalTime;
				hitTimer.value = hitAcc / timePerHit;
			}

			if (hitAcc > timePerHit)
			{
				hitAcc = 0;
				if (DoCircleCheck())
				{
					successCount += 1;
					ShowFeedback();
					Debug.Log("빻");
				}
			}

			if (accT > totalTime)
			{
				accT = 0;
				if (DoGameCheck())
				{
					EndGame();
				}
				else
				{
					FailGame();
				}
			}

			if (Input.GetMouseButtonDown(0))
			{
				spinMod *= -1;
			}
			indicator.transform.Rotate(Vector3.forward * spinMod * indicatorSpeed * Time.unscaledDeltaTime);
		}
		

	}

	public void SetRandAngle()
	{
		indicator.transform.rotation = Quaternion.Euler(Vector3.forward * Random.Range(-90, 90));
	}

	public void SetTargetAngle(float amt)
	{
		targetAngle = amt;
		target.fillAmount = amt / 360;
		target.transform.rotation = Quaternion.Euler(Vector3.forward * amt / 2);
	}

	public override void EndGame()
	{
		base.EndGame();
		PreProcess.ApplyProcessTo(ProcessType.Mash, minigameTarget);
	}


	public bool DoCircleCheck()
	{
		return Vector3.Dot(Vector3.down, indicator.transform.up) >= Mathf.Cos(targetAngle / 2 * Mathf.Deg2Rad);
	}

	public override bool DoGameCheck()
	{
		return sucsThreshold <= successCount;
	}
}
