using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireStrongArrow : Leaf
{
	public YinYang damage;
	public string shootPosName;
	public float angleY;

	public float scaleDiff;

	public float knockbackDist;
	//VisualEffect eff;
	Transform shootPos;

	private void OnValidate()
	{
		shootPos = GameObject.Find(shootPosName).transform;
		if (shootPos != null)
		{
			Debug.Log("FOUND!");
			//eff = shootPos.GetComponentInChildren<VisualEffect>();
		}
	}

	public override void UpdateStatus()
	{
		//
	}

	protected override void MyDisoperation(Actor self)
	{
		//
	}

	protected override void MyOperation(Actor self)
	{
		
	}
}
