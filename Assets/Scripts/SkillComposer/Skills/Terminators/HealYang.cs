using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Infos/HealYang")]
public class HealYang : Leaf
{

	public float amt;

	

	protected override void MyOperation(Actor self)
	{
		self.life.AddYY(amt, YYInfo.Yin);
		Debug.Log("양더함");
	}
	protected override void MyDisoperation(Actor self)
	{
		
	}
}