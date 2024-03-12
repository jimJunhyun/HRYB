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

	[SerializeField] protected Transform Owner;

	public LayerMask Layer => _layer;
	
	[Header("Already Get Object")][SerializeField] public Dictionary<Collider, bool> CheckDic = new();


	private bool _isRunning = false;
	
	
	private int _attackAbleCount = 0;
	
	
	public abstract Collider[] ReturnColliders();
	
	public Action<LifeModule> CastAct;
	
	protected void Update()
	{
		//Debug.LogError("업데이트 돌긴함");
		if (_isRunning == false)
			return;
		
		if(_attackAbleCount > 0 && CheckDic.Count > _attackAbleCount)
			return;
		
		
		//Debug.LogError("업데이트 들어옴");
		// 생각해 봤는데 어차피 col있는 만큼만 돌아가기 때문에 큰 문제 없음
		foreach (var col in ReturnColliders())
		{
			
			Debug.LogWarning($"{col.name} 맞음");
			if (CheckDic.ContainsKey(col))
				return;
			else
			{
				CheckDic.Add(col, false);
				Debug.LogError(col.name);
			}
			
			if (col.TryGetComponent<LifeModule>(out LifeModule lf))
			{
				CastAct?.Invoke(lf);
			}
			
		}
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	public void Now(Transform _owner, Action<LifeModule> act = null, int attackAble = -1, float StartSec = -1, float EndSec = -1, float dieSec = -1)
	{
		this.Owner = _owner;
		
		_attackAbleCount = attackAble;
		if(StartSec > 0)
		{
			StartCoroutine(StartSet(StartSec));
		}
		else
		{
			CheckDic.Clear();
			Debug.LogError("시작");
			_isRunning = true;
			if (act != null)
				CastAct = act;
		}

		if(EndSec > 0)
		{
			StartCoroutine(EndSet(EndSec));
		}

		if(dieSec > 0)
		{
			StartCoroutine(DieSet(dieSec));
		}
	}

	public void End()
	{
		_isRunning = false;
		
		CheckDic.Clear();
		CastAct = null;
		
		PoolManager.ReturnObject(gameObject);
	}

	IEnumerator StartSet(float t, Action<LifeModule> act = null)
	{
		yield return new WaitForSeconds(t);
		CheckDic.Clear();
		_isRunning = true;
		if (act != null)
			CastAct = act;
	}

	IEnumerator EndSet(float t)
	{
		yield return new WaitForSeconds(t);
		End();
	}

	IEnumerator DieSet(float t)
	{
		yield return new WaitForSeconds(t);
		try
		{
			PoolManager.ReturnObject(gameObject);
		}
		catch
		{
			Destroy(gameObject);
		}
	}
}
