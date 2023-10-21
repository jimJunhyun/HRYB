using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageObject : MonoBehaviour
{
    public YinyangWuXing yywx {private get; set;}
    public float effT {private get; set;}
	public bool isDirect { private get; set;}

	public virtual void OnTriggerEnter(Collider other)
	{
		LifeModule yc;
		if (other.TryGetComponent<LifeModule>(out yc))
		{
			Damage(yc);
		}
	}

	public virtual void SetInfo(YinyangWuXing y, float time, bool isDrt)
	{
		yywx = y;
		effT = time;
		isDirect = isDrt;
	}

	public virtual void Damage(LifeModule to)
	{
		if (isDirect)
		{
			to.AddYYWX(yywx);
		}
		else
		{
			to.AddYYWX(yywx, effT);
		}
	}
}
