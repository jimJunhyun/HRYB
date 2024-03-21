using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class YGComboAttackBase : AttackBase
{
	protected Action _nextTo = null;
	protected Action _cancel = null;
	protected ColliderCast _cols = null;
	
	public virtual void isAction(Action  t = null, Action v = null)
	{
		_nextTo = t;
		_cancel = v;
	}

    internal override void MyOperation(Actor self)
    {
    }

    internal override void MyDisoperation(Actor self)
    {
    }

    public override void UpdateStatus()
    {
	    
    }
}
