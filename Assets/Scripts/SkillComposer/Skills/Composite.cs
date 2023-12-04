using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Skills/Composite")]

public class Composite : Compose, IComposer
{
	public List<Compose> childs;
	public float composeDel;

	public void AddChild(Compose comp)
	{
		childs.Add(comp);
	}

	public override void Disoperate(Actor self)
	{
		GameManager.instance.StartCoroutine(DelDisoperate(self));
	}

	public override void Operate(Actor self)
	{
		GameManager.instance.StartCoroutine(DelOperate(self));
	}

	protected override void MyDisoperation(Actor self)
	{
		//Do nothing?
	}

	protected override void MyOperation(Actor self)
	{
		//Do nothing?
	}

	public override void UpdateStatus()
	{
		for (int i = 0; i < childs.Count; i++)
		{
			childs[i].UpdateStatus();
		}
	}

	IEnumerator DelOperate(Actor self)
	{
		for (int i = 0; i < childs.Count; i++)
		{
			childs[i].Operate(self);
			yield return new WaitForSeconds(composeDel);
		}
	}

	IEnumerator DelDisoperate(Actor self)
	{
		for (int i = 0; i < childs.Count; i++)
		{
			childs[i].Disoperate(self);
			yield return new WaitForSeconds(composeDel);
		}
	}
}
