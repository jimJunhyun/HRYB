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
			Debug.Log(other);
			PoolManager.GetObject("Hit 26", other.transform.position + (Vector3.up * other.transform.localScale.magnitude * 0.5f), Quaternion.LookRotation(other.transform.forward));
			Damage(yc);
		}
	}

	public virtual void SetInfo(YinYang y)
	{
		yy = y;
	}

	public virtual void Damage(LifeModule to)
	{
		to.AddYY(yy, true);
	}
}
