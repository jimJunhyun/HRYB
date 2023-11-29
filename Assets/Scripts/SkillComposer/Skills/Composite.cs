using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Skills/Composite")]

public class Composite : Compose, IComposer
{
	public List<Compose> childs;

	public void AddChild(Compose comp)
	{
		childs.Add(comp);
	}

	public override void Disoperate(Actor self)
	{
		for (int i = 0; i < childs.Count; i++)
		{
			childs[i].Disoperate(self);
		}
	}

	public override void Operate(Actor self)
	{
		for (int i = 0; i < childs.Count; i++)
		{
			childs[i].Operate(self);
		}
	}

	protected override void MyDisoperation(Actor self)
	{
		//Do nothing?
	}

	protected override void MyOperation(Actor self)
	{
		//Do nothing?
	}
}
