using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Infos/ExplodeFoxFire")]
public class ExplodeFoxFire : AttackBase
{
	public override void Operate(Actor self)
	{
		//base.Operate(self);
	}

	public override void Disoperate(Actor self)
	{
		//base.Disoperate(self);
	}
	public override void UpdateStatus()
	{
		
	}

	internal override void MyDisoperation(Actor self)
	{
		
	}

	internal override void MyOperation(Actor self)
	{
		if(GameManager.instance.foxfire.Mode == FoxFireMode.Attatched)
		{
			GameManager.instance.foxfire.Explode();
			PoolManager.GetObject("Lvl up", GameManager.instance.foxfire.transform.position, Quaternion.identity, 0.5f);
		}
	}
}
