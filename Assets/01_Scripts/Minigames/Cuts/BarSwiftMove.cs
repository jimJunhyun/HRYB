using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarSwiftMove : MonoBehaviour
{
    public float barMoveSpeed;

	public Vector2 barRandomJitter;
	
	Scrollbar scr;

	float accTime = 0;
	bool started;

	private void Awake()
	{
		scr = GetComponent<Scrollbar>();
		accTime = 0;
		ChangeSpeed();
		started = false;
	}

	private void Update()
	{
		if (started)
		{
			scr.value = Mathf.Lerp(0, 1, (Mathf.Sin(accTime) * 0.5f + 0.5f));
			accTime += Time.unscaledDeltaTime * barMoveSpeed;
		}
	}

	public void ChangeSpeed()
	{
		barMoveSpeed = Random.Range(barRandomJitter.x, barRandomJitter.y);
	}

	public void DoStart()
	{
		started = true;
	}
}
