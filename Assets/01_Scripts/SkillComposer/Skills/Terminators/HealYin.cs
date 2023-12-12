using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Infos/HealYin")]
public class HealYin : Leaf
{
    
	public float amt;

	internal override void MyDisoperation(Actor self)
	{

	}

	internal override void MyOperation(Actor self)
	{
		self.life.AddYY(amt, YYInfo.Yin);
		Debug.Log("음더함");
	}
	public override void UpdateStatus()
	{
		throw new System.NotImplementedException();
	}
}
