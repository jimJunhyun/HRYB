using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Infos/EnhanceSelf")]
public class EnhanceSelf : Leaf
{
	public List<StatusEffectApplyData> data;

	public override void UpdateStatus()
	{
		
	}

	internal override void MyDisoperation(Actor self)
	{
		//해제하는가?
	}

	internal override void MyOperation(Actor self)
	{
		for (int i = 0; i < data.Count; i++)
		{
			StatusEffects.ApplyStat(self, self, data[i].id, data[i].duration, level);
		}
	}
}
