using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Infos/Kick")]
public class Kick : AttackBase
{
	BoxColliderCast caster;

	protected override void OnValidate()
	{
		base.OnValidate();
		caster = relatedTransform.GetComponent<BoxColliderCast>();
		if(caster == null)
		{
			Debug.Log($"NO BOXCOLLIDERCAST FOUND IN : {relatedTransform.name}");
		}
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
		caster.Now(life =>
		{
			if(life != null)
			{
				DoDamage(life.GetActor(), self);
				GameManager.instance.ShakeCamFor(0.1f);
				PoolManager.GetObject("Hit 26", life.transform.position + (Vector3.up * life.transform.localScale.magnitude * 0.5f), Quaternion.LookRotation(life.transform.forward));
			}
			
		});
	}
}
