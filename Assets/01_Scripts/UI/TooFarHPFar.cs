using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooFarHPFar : MonoBehaviour
{
	[Header("HP바 감지범위")]
	public int range;

	GameObject player;
	private float pRange;
	GameObject obj;
	HPBar bar;

	private void Start()
	{
		obj = this.gameObject.GetComponentInParent<LifeModule>().gameObject;

		player = GameManager.instance.player;
		bar = GetComponentInChildren<HPBar>();
	}

	private void Update()
	{

		if (Vector3.Distance(this.gameObject.transform.position, player.transform.position) < range)
		{
			bar.gameObject.SetActive(true);

		}
		else
		{
			bar.gameObject.SetActive(false);
		}

		pRange = Vector3.Distance(obj.transform.position, player.transform.position);
	}
}
