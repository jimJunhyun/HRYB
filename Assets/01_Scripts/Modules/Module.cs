using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module : MonoBehaviour
{
	protected Actor self;
    public virtual Actor GetActor()
	{
		if(self == null)
		{
			return self = GetComponent<Actor>();
		}
		else
		{
			return self;
		}
	}

	public virtual void ResetStatus()
	{

	}
}
