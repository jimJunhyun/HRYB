using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InterPoint : MonoBehaviour, IInterable
{
	public bool isInterable = true;
	public float interTime = 0.3f;
	public bool altInterable = false;

	public UnityEvent onInter;
	public UnityEvent onAltInter;


	public bool IsInterable { get => isInterable; set => isInterable = value; }
	public float InterTime { get => interTime; set => interTime = value; }

	public bool AltInterable => altInterable;

	Renderer r;
	Material mat;
	//Coroutine ongoing;

	private void Awake()
	{
		r = GetComponent<Renderer>();
		mat = r.material;
		r.material = new Material(mat);
		mat = r.material;
		GlowOff();
	}

	public void GlowOff()
	{
		mat.SetFloat(IInterable.GlowPowerHash, 0f);
	}

	public void GlowOn()
	{
		mat.SetFloat(IInterable.GlowPowerHash, 0.5f);
	}

	public void InteractWith()
	{
		Inter();
	}

	public virtual void Inter()
	{
		onInter.Invoke();
	}

	public void AltInterWith()
	{
		AltInter();
	}

	public virtual void AltInter()
	{
		onAltInter.Invoke();
	}
}
