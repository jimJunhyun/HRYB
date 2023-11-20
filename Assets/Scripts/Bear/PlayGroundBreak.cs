using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlayGroundBreak : MonoBehaviour
{
    public float rangeZ =  15;
	public float moveSec = 0.5f;
	VisualEffect effect;

	Coroutine ongoing;

	private void Awake()
	{
		effect = GetComponentInChildren<VisualEffect>();
		effect.Stop();
	}

	public void PlayEffect()
	{
		if(ongoing == null)
		{
			GameManager.instance.StartCoroutine(DelMove());
		}
	}

	IEnumerator DelMove()
	{
		float t= 0;
		Vector3 initPos = transform.position;
		effect.Play();
		while(t < moveSec)
		{
			t += Time.deltaTime;
			yield return null;
			transform.position = Vector3.Lerp(initPos, initPos + (transform.forward * rangeZ), t / moveSec);
		}
		effect.Stop();
		ongoing = null;
		transform.position = initPos;
	}
}
