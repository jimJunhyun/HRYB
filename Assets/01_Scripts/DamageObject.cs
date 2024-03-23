using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageObject : MonoBehaviour
{
    public YinYang yy;

	public virtual void OnTriggerEnter(Collider other)
	{
		LifeModule yc;
		if (other.TryGetComponent<LifeModule>(out yc))
		{
			//Debug.Log(other);
			Vector3 hitPos = other.ClosestPointOnBounds(transform.position);
			PoolManager.GetObject("Hit 26", hitPos, Quaternion.LookRotation(other.transform.forward), 2.5f);
			Damage(yc);
			if (yy.white > 0)
			{
				GameManager.instance.shower.GenerateDamageText(hitPos, yy.white, YYInfo.White);
			}
			if (yy.black > 0)
			{
				GameManager.instance.shower.GenerateDamageText(hitPos, yy.black, YYInfo.Black);
			}
		}
	}

	public virtual void SetInfo(YinYang y)
	{
		yy = y;
	}

	public virtual void Damage(LifeModule to)
	{
		to.DamageYY(yy, DamageType.DirectHit);
	}
}
