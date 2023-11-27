using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Compose : ScriptableObject, IComposer
{
	public abstract void Operate();

	protected abstract void MyOperation();
}
