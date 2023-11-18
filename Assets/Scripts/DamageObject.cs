using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageObject : MonoBehaviour
{
    public YinyangWuXing yywx ;
    public float effS;
	public bool isDirect ;

	public virtual void OnTriggerEnter(Collider other)
	{
		LifeModule yc;
		if (other.TryGetComponent<LifeModule>(out yc))
		{
			Debug.Log(other);
			Damage(yc);
		}
	}

	public virtual void SetInfo(YinyangWuXing y, float time, bool isDrt)
	{
		yywx = y;
		effS = time;
		isDirect = isDrt;
	}

	public virtual void Damage(LifeModule to)
	{
		if (isDirect)
		{
			to.AddYYWX(yywx, true);
		}
		else
		{
			to.AddYYWX(yywx, effS, true);
		}
	}
}
