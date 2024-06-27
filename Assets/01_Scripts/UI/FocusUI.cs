using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AdditionalEffectFocusing
{
	None = 0,
	Border = 1,
	Arrow = 2,

}

public class FocusUI : MonoBehaviour
{
    RectTransform shade;
	RectTransform foci;

	Image border;
	Image arrow;

	
	Canvas c;
	Coroutine ongoing;
	Coroutine bouncer;

	bool focusing;
	bool bouncing;
	AdditionalEffectFocusing focusMode;

	const float LERPSPEED = 0.3f;
	const float BOUNCEAMOUNT = 0.2f;
	const float BOUNCESPEED = 6f;

	private void Awake()
	{
		foci = transform.Find("Foci").GetComponent<RectTransform>();
		shade = transform.Find("Foci/Shade").GetComponent<RectTransform>();
		border = transform.Find("Foci/Border").GetComponent<Image>();
		arrow = transform.Find("Arrow").GetComponent<Image>();
		c = GetComponent<Canvas>();
	}
	private void Start()
	{
		OffShade(false);
	}

	private void Update()
	{
		FocusAt(new Rect(Input.mousePosition, Vector2.one * 300), false, AdditionalEffectFocusing.Arrow);
		DoBounce();
	}

	public void OnShade()
	{
		c.enabled = true;
	}

	public void OffShade(bool isLerping)
	{
		if(bouncer != null)
		{
			StopCoroutine(bouncer);
		}

		if (isLerping)
		{
			if(ongoing != null)
				StopCoroutine(ongoing);
			ongoing = StartCoroutine(DelLerpFociOff());
		}
		else
		{
			c.enabled = false;
			focusing = false;
			focusMode = AdditionalEffectFocusing.None;
		}
	}

	public void FocusAt(RectTransform rt, bool isLerping, AdditionalEffectFocusing eff)
	{
		OnShade();

		switch (eff)
		{
			case AdditionalEffectFocusing.None:
				border.enabled = false;
				arrow.enabled = false;
				break;
			case AdditionalEffectFocusing.Border:
				border.enabled = true;
				arrow.enabled = false;
				break;
			case AdditionalEffectFocusing.Arrow:
				border.enabled = false;
				arrow.enabled = true;
				break;
			case AdditionalEffectFocusing.Border | AdditionalEffectFocusing.Arrow:
				border.enabled = true;
				arrow.enabled = true;
				break;
		}
		if (isLerping)
		{
			if(ongoing != null)
				StopCoroutine(ongoing);
			ongoing = StartCoroutine(DelLerpFociOn(rt.rect));
		}
		else
		{
			foci.position = rt.position;
			foci.sizeDelta = rt.sizeDelta;
			shade.position = transform.position;
			focusing = true;
		}
		focusMode = eff;
		
	}

	public void FocusAt(Rect rt, bool isLerping, AdditionalEffectFocusing eff)
	{
		OnShade();

		switch (eff)
		{
			case AdditionalEffectFocusing.None:
				border.enabled = false;
				arrow.enabled = false;
				break;
			case AdditionalEffectFocusing.Border:
				border.enabled = true;
				arrow.enabled = false;
				break;
			case AdditionalEffectFocusing.Arrow:
				border.enabled = false;
				arrow.enabled = true;
				break;
			case AdditionalEffectFocusing.Border | AdditionalEffectFocusing.Arrow:
				border.enabled = true;
				arrow.enabled = true;
				break;
		}
		if (isLerping)
		{
			if (ongoing != null)
				StopCoroutine(ongoing);
			ongoing = StartCoroutine(DelLerpFociOn(rt));
		}
		else
		{
			foci.position = new Vector3(rt.x, rt.y);
			foci.sizeDelta = new Vector2(rt.width, rt.height);
			shade.position = transform.position;
			focusing = true;
		}
		focusMode = eff;
	}

	IEnumerator DelLerpFociOn(Rect target)
	{
		float t = 0;
		Vector2 res = new Vector2(Screen.width, Screen.height);
		Rect origin = new Rect(res * 0.5f, res);
		while(t <= 1)
		{
			yield return null;
			t += LERPSPEED * Time.deltaTime;
			foci.position = Vector3.Lerp(new Vector3(origin.x, origin.y), new Vector3(target.x, target.y), t);
			foci.sizeDelta = Vector2.Lerp(new Vector3(origin.width, origin.height), new Vector3(target.width, target.height), t);
			shade.position = transform.position;
		}
		focusing = true;
	}

	IEnumerator DelLerpFociOff()
	{
		float t = 0;
		Rect origin = foci.rect;
		Vector2 res = new Vector2(Screen.width, Screen.height);
		Rect target = new Rect(res * 0.5f, res);
		while (t <= 1)
		{
			yield return null;
			t += LERPSPEED * Time.deltaTime;
			foci.position = Vector3.Lerp(new Vector3(origin.x, origin.y), new Vector3(target.x, target.y), t);
			foci.sizeDelta = Vector2.Lerp(new Vector3(origin.width, origin.height), new Vector3(target.width, target.height), t);
			shade.position = transform.position;
		}
		focusing = false;
		c.enabled = false;
	}

	IEnumerator DelBounceFoci()
	{
		yield return new WaitUntil(()=>focusing);
		Rect origin = foci.rect;
		float accT = 0;
		while(true)
		{
			yield return null;
			accT += Time.deltaTime;
			float t = 1 + Mathf.Abs(Mathf.Sin(accT * BOUNCESPEED)) * BOUNCEAMOUNT;
			Rect rt = new Rect(new Vector2(origin.x, origin.y), new Vector2(origin.width, origin.height));
			rt.width *= t;
			rt.height *= t;
			foci.sizeDelta = new Vector2(rt.width, rt.height);

			shade.position = transform.position;
		}
	}

	public void DoBounce()
	{
		bouncing = true;
		if (bouncer == null)
		{
			bouncer = StartCoroutine(DelBounceFoci());
		}
	}

	public void StopBounce()
	{
		bouncing = false;
		if(bouncer != null)
		{
			StopCoroutine(bouncer);
		}
	}

}
