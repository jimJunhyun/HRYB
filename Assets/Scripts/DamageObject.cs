using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageObject : MonoBehaviour
{
    public YinyangWuXing yywx;

	public virtual void OnTriggerEnter(Collider other)
	{
		YinyangCreature yc;
		if (other.TryGetComponent<YinyangCreature>(out yc))
		{
			Damage(yc);
		}
	}

	public virtual void Damage(YinyangCreature to)
	{
		to.AddYYWX(yywx);
	}
}
