using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InterPoint : MonoBehaviour, IInterable
{
	public bool isInterable = true;
	public float interTime = 0.3f;

	public UnityEvent onInter;

	public bool IsInterable { get => isInterable; set => isInterable = value; }
	public float InterTime { get => interTime; set => interTime = value; }

	Renderer r;
	Material mat;
	Coroutine ongoing;

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
		if (ongoing == null)
		{
			ongoing = StartCoroutine(DelInter());
		}
	}

	public IEnumerator DelInter()
	{
		GameManager.instance.pinp.DeactivateInput();
		float t = 0;
		while (t < InterTime)
		{
			t += Time.deltaTime;
			yield return null;
		}
		
		onInter.Invoke();

		GameManager.instance.pinp.ActivateInput();
		ongoing = null;
	}
}
