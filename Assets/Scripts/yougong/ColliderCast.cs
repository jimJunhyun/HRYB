using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ColliderCast : MonoBehaviour
{

	//[Header("Collider Name")] [SerializeField]
	//private string _name;
//
	//public string Name => _name;
	
	[Header("Enemy Layer")]
	[SerializeField] private LayerMask _layer;

	public LayerMask Layer => _layer;
	
	[Header("Already Get Object")][SerializeField] public Dictionary<Collider, bool> CheckDic = new();


	private bool _isRunning = false;
	
	
	public abstract Collider[] ReturnColliders();
	
	public Action<LifeModule> CastAct;
	
	protected void Update()
	{
		if (_isRunning == false)
			return;
		
		// 생각해 봤는데 어차피 col있는 만큼만 돌아가기 때문에 큰 문제 없음
		foreach (var col in ReturnColliders())
		{
			if (CheckDic.ContainsKey(col))
				return;
			else
				CheckDic.Add(col, false);
			
			CastAct?.Invoke(col.GetComponent<LifeModule>());
			Debug.Log($"{col.name} 맞음");

		}
	}

	public void Now(Action<LifeModule> act = null)
	{
		_isRunning = true;
		CastAct = act;
	}

	public void End()
	{
		_isRunning = false;
		
		CheckDic.Clear();
		CastAct = null;
	}
}
