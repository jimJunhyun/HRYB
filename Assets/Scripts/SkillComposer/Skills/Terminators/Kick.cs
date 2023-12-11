using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Infos/Kick")]
public class Kick : Leaf
{
	public YinYang damage;
	public string casterName;
	public float knockbackPow;

	BoxColliderCast caster;

	private void OnValidate()
	{
		caster = GameObject.Find("KickCaster").GetComponent<BoxColliderCast>();
	}

	public override void Operate(Actor self)
	{

	}

	public override void UpdateStatus()
	{
		
	}

	internal override void MyDisoperation(Actor self)
	{
		caster.End();
	}

	internal override void MyOperation(Actor self)
	{
		Debug.Log("KICKED");
		caster.Now(life =>
		{
			if(life != null)
			{
				life.AddYY(damage);
				StatusEffects.ApplyStat(life.GetActor(), self, StatEffID.Knockback, 0.2f, knockbackPow);
			}
			
		});
	}
}
