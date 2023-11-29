using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageObject : MonoBehaviour
{
    public YinYang yy ;
    public float effS;
	public bool isDirect ;

	public virtual void OnTriggerEnter(Collider other)
	{
		LifeModule yc;
		if (other.TryGetComponent<LifeModule>(out yc))
		{
			Debug.Log(other);
			PoolManager.GetObject("DamagedEffect", other.transform.position + (Vector3.up * other.transform.localScale.magnitude * 0.5f), Quaternion.LookRotation(other.transform.forward));
			Damage(yc);
		}
	}

	public virtual void SetInfo(YinYang y, float time, bool isDrt)
	{
		yy = y;
		effS = time;
		isDirect = isDrt;
	}

	public virtual void Damage(LifeModule to)
	{
		if (isDirect)
		{
			to.AddYY(yy, true);
		}
		else
		{
			to.AddYY(yy, effS, true);
		}
	}
}
