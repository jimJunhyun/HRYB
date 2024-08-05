using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleLoader : MonoBehaviour
{
	TMP_Text titleText;
	CanvasGroup canvasGroup;

	public bool isFade = false;
	float fadeInOutTime;

	private void Awake()
	{
		titleText = GetComponentInChildren<TMP_Text>();
		canvasGroup = GetComponent<CanvasGroup>();
	}

	void Start()
	{
		canvasGroup.alpha = 0f;
	}

	public void FadeStop()
	{
		StopAllCoroutines();
		isFade = false;
		titleText.text = "";
	}

	public void FadeInOut(string text, float fadeIn = 0.8f, float fadeOut = 0.8f, float holdTime = 1f)
	{
		if (isFade) return;
		isFade = true;
		titleText.text = text;

		GameManager.instance.audioPlayer.PlayPoint("TextOn", transform.position);

		StartCoroutine(FadeInOutRoutine(fadeIn, fadeOut,holdTime));
	}

	IEnumerator FadeInOutRoutine(float fadeIn, float fadeOut, float holdTime)
	{
		float elapsedTime = fadeIn;

		// Fade In
		while (elapsedTime <= fadeInOutTime)
		{
			elapsedTime += Time.deltaTime;
			canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeInOutTime);
			yield return null;
		}

		canvasGroup.alpha = 1f;

		// Wait for a moment
		yield return new WaitForSeconds(holdTime);

		// Fade Out
		elapsedTime = fadeOut; // Reset elapsed time for fade out
		while (elapsedTime >= 0f)
		{
			elapsedTime -= Time.deltaTime;
			canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeInOutTime);
			yield return null;
		}

		canvasGroup.alpha = 0f;
		isFade = false;
	}
}
