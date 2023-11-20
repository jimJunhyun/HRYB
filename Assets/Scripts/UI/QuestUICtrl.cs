using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestUICtrl : MonoBehaviour
{
    public	TextMeshProUGUI text;
	public float xMove;
	public float xMoveTime;

	bool isOn = false;

	Coroutine ongoing;

	private void Awake()
	{
		 text = GetComponentInChildren<TextMeshProUGUI>();
	}

	public void On()
	{
		if(ongoing == null && !isOn)
		ongoing=  StartCoroutine(DelMove(false));
	}

	public void Off()
	{
		if(ongoing == null && isOn)
		ongoing = StartCoroutine(DelMove(true));
	}

	public void SetText(string str)
	{
		text.text = str;
	}

	IEnumerator DelMove(bool isRight)
	{
		float t = 0;
		Vector3 initPos = transform.position;
		Vector3 destPos;
		if (isRight)
		{
			destPos = initPos + Vector3.right * xMove;
		}
		else
		{
			destPos = initPos + Vector3.left * xMove;
		}
		while(t < xMoveTime)
		{
			yield return null;
			t += Time.deltaTime;
			transform.position = Vector3.Lerp(initPos, destPos, t / xMoveTime);	
		}
		ongoing = null;
	}

}
