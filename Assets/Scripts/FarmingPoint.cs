using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmingPoint : MonoBehaviour, IInterable
{
	const float RECENTERINGTIME = 0.2f;


	public float interTime = 1.0f;
	public bool isInterable = true;
	public bool isDestroyed = true;

	public string resItem;
	public int amount;

	public bool IsInterable { get => isInterable; set => isInterable = value; }
	public float InterTime { get => interTime; set => interTime = value; }

	Renderer r;
	Material mat;

	private static readonly int GlowPowerHash = Shader.PropertyToID("_GlowPower");
	Coroutine ongoing;

	private void Awake()
	{
		r = GetComponent<Renderer>();
		mat = r.material;
		r.material = new Material(mat);
		mat = r.material;
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
			ongoing = GameManager.instance.StartCoroutine(DelInter()); //임시. 풀링으로 변경하면 그냥 여기서 하면 됨/
		}
	}

	void Inter()
	{
		bool success = false;
		if (Item.nameHashT.ContainsKey(resItem.GetHashCode()))
		{
			success = GameManager.instance.pinven.AddItem((Item.nameHashT[resItem.GetHashCode()] as Item), amount);
		}
		if (success)
		{
			Debug.Log(transform.name);
			if (isDestroyed)
			{
				Destroy(gameObject); //임시. 이후 풀링으로 변경.
			}
		}
		
	}

	IEnumerator DelInter()
	{
		GameManager.instance.pinp.DeactivateInput();
		GameManager.instance.pCam.m_XAxis.m_Wrap = true;
		float t = 0;
		while(t < InterTime)
		{
			t += Time.deltaTime;
			yield return null;
		}
		Inter();
		t = 0;
		float fromVal = GameManager.instance.pCam.m_XAxis.Value;
		while (t < RECENTERINGTIME)
		{
			t += Time.deltaTime;
			yield return null;
			GameManager.instance.pCam.m_XAxis.Value = Mathf.Lerp(fromVal, 0, t / RECENTERINGTIME);
		}
		GameManager.instance.pinp.ActivateInput();
		GameManager.instance.pCam.m_XAxis.m_Wrap = false;
		
		ongoing = null;
	}
}
