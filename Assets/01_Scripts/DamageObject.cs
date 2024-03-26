using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageObject : MonoBehaviour
{
    public YinYang yy;
	protected List<StatusEffectApplyData> statData = new List<StatusEffectApplyData>();

	protected Actor owner;

	public virtual void OnTriggerEnter(Collider other)
	{
		LifeModule yc;
		if (other.TryGetComponent<LifeModule>(out yc))
		{
			//Debug.Log(other);
			Vector3 hitPos = other.ClosestPointOnBounds(transform.position);
			PoolManager.GetObject("Hit 26", hitPos, Quaternion.LookRotation(other.transform.forward), 2.5f);
			Damage(yc);
			for (int i = 0; i < statData.Count; i++)
			{
				StatusEffects.ApplyStat(yc.GetActor(), owner, statData[i].id, statData[i].duration, statData[i].power);
			}
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

	public virtual void SetInfo(YinYang y, List<StatusEffectApplyData> data, Actor self)
	{
		yy = y;
		statData = data;
		owner = self;
	}

	public virtual void Damage(LifeModule to)
	{
		to.DamageYY(yy, DamageType.DirectHit);
	}
}
