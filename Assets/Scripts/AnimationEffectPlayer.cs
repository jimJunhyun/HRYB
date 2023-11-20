using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AnimationEffectPlayer : MonoBehaviour
{
	VisualEffect effect;

	private void Awake()
	{
		effect = GetComponentInChildren<VisualEffect>();
		effect.Stop();
	}

	public void PlayEffect()
	{
		effect.Reinit();
		effect.Play();
	}

	public void Rotate180()
	{
		transform.eulerAngles += new Vector3(0, 180, 0);
	}
}
