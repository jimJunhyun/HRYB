using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OperateType
{
	None,
	Pre,
	Post,
	Both,
}

public interface IComposer
{
    public void Operate(Actor self);
    public void Disoperate(Actor self);
}
