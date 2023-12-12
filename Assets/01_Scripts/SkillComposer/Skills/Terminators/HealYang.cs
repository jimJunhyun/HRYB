using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Infos/HealYang")]
public class HealYang : Leaf
{

	public float amt;

	

	internal override void MyOperation(Actor self)
	{
		self.life.AddYY(amt, YYInfo.Yang);
		Debug.Log("양더함");
	}
	internal override void MyDisoperation(Actor self)
	{
		
	}

	public override void UpdateStatus()
	{
		throw new System.NotImplementedException();
	}
}