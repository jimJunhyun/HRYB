using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooFarHPFar : MonoBehaviour
{
	[Header("HP바 감지범위")]
	public int Range;

	GameObject obj;

	[Header("감지할 대상의 레이어")]
	public LayerMask layer;

	private SphereCollider Collider;

	private LifeModule lf;

	private void Awake()
	{
		Collider = GetComponent<SphereCollider>();
		lf = GetComponent<LifeModule>();
	}

	private void FixedUpdate()
	{
		Collider.radius = Range;
	}

	private void OnTriggerEnter(Collider other)
	{

		if (other.gameObject.layer == 7)
		{
			obj = GameManager.instance.bHPManager.HideHP(this.transform);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == 7)
		{
			Destroy(obj);
		}
	}
}
