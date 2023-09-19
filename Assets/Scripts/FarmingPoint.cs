using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmingPoint : MonoBehaviour, IInterable
{
	public float interTime = 1.0f;
	public bool isInterable = true;
	public bool isDestroyed = true;

	public bool IsInterable { get => isInterable; set => isInterable = value; }
	public float InterTime { get => interTime; set => interTime = value; }

	Renderer r;
	Material mat;

	private static readonly int GlowPowerHash = Shader.PropertyToID("_GlowPower");
	WaitForSeconds wait;
	Coroutine ongoing;

	private void Awake()
	{
		r = GetComponent<Renderer>();
		mat = r.material;
		r.material = new Material(mat);
		mat = r.material;
		wait = new WaitForSeconds(interTime);
		GlowOff();
	}

	public void GlowOn()
	{
		mat.SetFloat(GlowPowerHash, 0.5f);
		
	}

	public void GlowOff()
	{
		mat.SetFloat(GlowPowerHash, 0f);
	}

	public void InteractWith()
	{
		if(ongoing == null)
		{
			ongoing = StartCoroutine(DelInter());
		}
	}

	void Inter()
	{
		Debug.Log(transform.name);
		if (isDestroyed)
		{
			Destroy(gameObject); //임시. 이후 풀링으로 변경.
		}
	}

	IEnumerator DelInter()
	{
		GameManager.instance.pinp.DeactivateInput();
		yield return wait;
		GameManager.instance.pinp.ActivateInput();
		Inter();
		ongoing = null;
	}
}
