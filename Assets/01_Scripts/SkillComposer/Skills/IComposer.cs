using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IComposer
{
    public void Operate(Actor self);
    public void Disoperate(Actor self);
}
