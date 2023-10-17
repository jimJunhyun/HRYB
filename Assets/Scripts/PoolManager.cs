using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
	PoolList list;

	public void Awake()
	{
		list = Resources.Load<PoolList>("PoolList");
	}
}
