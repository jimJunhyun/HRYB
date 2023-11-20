using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelReturnSelf : MonoBehaviour
{
    public float returnTime = 0.5f;
	WaitForSeconds sec;
	private void Awake()
	{
		sec = new WaitForSeconds(returnTime);
	}
	private void OnEnable()
	{
		StartCoroutine(DelReturn());
		
	}
	IEnumerator DelReturn()
	{
		yield return sec;
		PoolManager.ReturnObject(gameObject);
	}
}
