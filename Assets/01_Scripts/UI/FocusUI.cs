using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Flags]
public enum AdditionalEffectFocusing
{
	None = 0,
	Border = 1,
	Arrow = 2,
	Text = 4,
	Subtitle = 8,
	Bounce = 16,

}

public class FocusUI : MonoBehaviour
{
    RectTransform shade;
	RectTransform foci;

	Image border;
	Image arrow;
	TextMeshProUGUI explain;
	TextMeshProUGUI subtitle;

	
	Canvas c;
	Coroutine ongoing;
	Coroutine bouncer;

	bool focusing;
	bool bouncing;
	AdditionalEffectFocusing focusMode;

	const float LERPSPEED = 0.7f;
	const float BOUNCEAMOUNT = 0.2f;
	const float BOUNCESPEED = 6f;

	private void Awake()
	{
		foci = transform.Find("Foci").GetComponent<RectTransform>();
		shade = transform.Find("Foci/Shade").GetComponent<RectTransform>();
		border = transform.Find("Foci/Border").GetComponent<Image>();
		arrow = transform.Find("Arrow").GetComponent<Image>();
		explain = transform.Find("Explain").GetComponent<TextMeshProUGUI>();
		subtitle = transform.Find("Subtitle").GetComponent<TextMeshProUGUI>();
		c = GetComponent<Canvas>();
	}
	private void Start()
	{
		OffShade(false);
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

		border.enabled =false;
		arrow.enabled =false;
		explain.enabled =false;
		subtitle.enabled =false;

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
			bouncing =false;
			focusMode = AdditionalEffectFocusing.None;
		}
	}

	public void FocusAt(RectTransform rt, bool isLerping, AdditionalEffectFocusing eff)
	{
		OnShade();


		border.enabled = eff.HasFlag(AdditionalEffectFocusing.Border);

		arrow.enabled = eff.HasFlag(AdditionalEffectFocusing.Arrow);

		explain.enabled = eff.HasFlag(AdditionalEffectFocusing.Text);

		subtitle.enabled = eff.HasFlag(AdditionalEffectFocusing.Subtitle);

		bouncing = eff.HasFlag(AdditionalEffectFocusing.Bounce);

		if (isLerping)
		{
			if(ongoing != null)
				StopCoroutine(ongoing);
			ongoing = StartCoroutine(DelLerpFociOn(rt.rect, rt.position));

		}
		else
		{
			foci.position = rt.position;
			foci.sizeDelta = rt.sizeDelta;
			shade.position = transform.position;
			focusing = true;

			if (bouncer != null)
				StopCoroutine(bouncer);
			if (bouncing)
			{
				bouncer = StartCoroutine(DelBounceFoci(rt.rect));
			}
		}
		focusMode = eff;
		
	}

	public void FocusAt(Rect rt, Vector2 pos, bool isLerping, AdditionalEffectFocusing eff)
	{
		OnShade();

		border.enabled = eff.HasFlag(AdditionalEffectFocusing.Border);

		arrow.enabled = eff.HasFlag(AdditionalEffectFocusing.Arrow);

		explain.enabled = eff.HasFlag(AdditionalEffectFocusing.Text);

		subtitle.enabled= eff.HasFlag(AdditionalEffectFocusing.Subtitle);

		bouncing = eff.HasFlag(AdditionalEffectFocusing.Bounce);

		if (isLerping)
		{
			if (ongoing != null)
				StopCoroutine(ongoing);
			ongoing = StartCoroutine(DelLerpFociOn(rt, pos));

		}
		else
		{
			foci.position = pos;
			foci.sizeDelta = new Vector2(rt.width, rt.height);
			shade.position = transform.position;
			focusing = true;

			if(bouncer != null)
				StopCoroutine(bouncer);
			if (bouncing)
			{
				bouncer = StartCoroutine(DelBounceFoci(rt));
			}
		}
		focusMode = eff;
	}

	public void SetExplain(string txt)
	{
		explain.text = txt;
	}

	public void SetSubTitle(string txt)
	{
		subtitle.text = txt;
	}

	IEnumerator DelLerpFociOn(Rect target, Vector2 pos)
	{
		float t = 0;
		Vector2 res = new Vector2(Screen.width, Screen.height);
		Rect origin = new Rect(res * 0.5f, res);
		if (bouncer != null)
			StopCoroutine(bouncer);
		//Debug.Log(pos + " 가 위치임.");
		while (t <= 1)
		{
			yield return null;
			t += LERPSPEED * Time.unscaledDeltaTime;
			foci.position = Vector3.Lerp(new Vector3(origin.x, origin.y), pos, t);
			//Debug.Log(foci.position + " 가 현재위치임 ㅐ ㅐ ㅐ ㅐ ㅐ .");
			foci.sizeDelta = Vector2.Lerp(new Vector3(origin.width, origin.height), new Vector3(target.width, target.height), t);
			shade.position = transform.position;
		}
		focusing = true;

		
		if (bouncing)
		{
			bouncer = StartCoroutine(DelBounceFoci(target));
		}
	}

	IEnumerator DelLerpFociOff()
	{
		float t = 0;
		if (bouncer != null)
			StopCoroutine(bouncer);
		Rect origin = foci.rect;
		Vector2 res = new Vector2(Screen.width, Screen.height);
		Rect target = new Rect(res * 0.5f, res);
		while (t <= 1)
		{
			yield return null;
			t += LERPSPEED * Time.unscaledDeltaTime;
			foci.position = Vector3.Lerp(new Vector3(origin.x, origin.y), new Vector3(target.x, target.y), t);
			foci.sizeDelta = Vector2.Lerp(new Vector3(origin.width, origin.height), new Vector3(target.width, target.height), t);
			shade.position = transform.position;
		}
		focusing = false;
		bouncing = false;
		c.enabled = false;
	}

	IEnumerator DelBounceFoci(Rect origin)
	{
		float accT = 0;
		while(true)
		{
			yield return null;
			accT += Time.unscaledDeltaTime;
			float t = 1 + Mathf.Abs(Mathf.Sin(accT * BOUNCESPEED)) * BOUNCEAMOUNT;
			Rect rt = new Rect(origin.position, new Vector2(origin.width, origin.height));
			rt.width *= t;
			rt.height *= t;
			foci.sizeDelta = new Vector2(rt.width, rt.height);

			shade.position = transform.position;
		}
	}

}
