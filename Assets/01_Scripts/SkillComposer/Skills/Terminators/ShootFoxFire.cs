using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Infos/ShootFoxFire")]
public class ShootFoxFire : AttackBase
{
	public float shootSpeed;

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
		GameManager.instance.foxfire.Fly(self.transform.forward, shootSpeed);
		GameManager.instance.audioPlayer.PlayPoint(audioClipName, self.transform.position);
	}
}
