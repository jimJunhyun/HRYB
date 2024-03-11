using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module : MonoBehaviour
{
	private Actor _self;
	protected Actor self
	{
		get
		{
			if(_self == null)
			{
				_self = GetComponent<Actor>();
			}
			return _self;
		}
	}

    public virtual Actor GetActor()
	{
		return self;
	}

	public virtual void ResetStatus()
	{

	}
}
