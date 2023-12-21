using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IComposer
{
	public void Operate(Actor self);
	public void Disoperate(Actor self);
}
