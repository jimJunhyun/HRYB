using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "New Skill Composite")]

public class Composite : Compose
{
	public List<Compose> childs;
	public OperateType operateType = OperateType.Pre;
	//public UnityAction act;

	public void AddChild(Compose comp)
	{
		childs.Add(comp);
	}

	public override sealed void Operate()
	{
		if(operateType == OperateType.Pre || operateType == OperateType.Both)
		{
			MyOperation();
		}

		for (int i = 0; i < childs.Count; i++)
		{
			childs[i].Operate();
		}

		if(operateType == OperateType.Post || operateType == OperateType.Both)
		{
			MyOperation();
		}
	}

	protected override void MyOperation()
	{
		
	}
}
