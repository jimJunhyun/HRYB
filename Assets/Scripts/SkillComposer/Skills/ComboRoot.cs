using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Skills/Combo")]
public class ComboRoot : SkillRoot
{
	public int initCombo;
	public float resetSec;
	public int resetThreshold;

	int curCombo = 0;
	float prevComboSec;

	public override void Disoperate(Actor self)
	{
		base.Disoperate(self);
	}

	public override void Operate(Actor self)
	{
		if (curCombo >= childs.Count)
		{
			ResetCombo();
			Debug.Log("콤보 최대, 초기화");
		}
		prevComboSec = Time.time;
		Debug.Log($"콤보 {curCombo}/{childs.Count}");
		childs[curCombo].Operate(self);
	}

	public void UpdateStatus()
	{
		if(resetSec	> resetThreshold && Time.time - prevComboSec >= resetSec && curCombo != 0)
		{
			Debug.Log("콤보유지시간초과");
			ResetCombo();
			prevComboSec = Time.time;
		}
	}

	protected override void MyDisoperation(Actor self)
	{
		//Do nothing
	}

	protected override void MyOperation(Actor self)
	{
		//Do nothing
	}

	public void NextCombo(bool circular = true, Actor self = null)
	{
		
		if(self != null)
			childs[curCombo].Disoperate(self);
		
		if (circular)
		{
			prevComboSec = Time.time;
			Debug.Log("다음콤보");
			curCombo += 1;
			curCombo %= childs.Count;
		}
		else if(curCombo < childs.Count)
		{
			prevComboSec = Time.time;
			Debug.Log("다음콤보");
			curCombo += 1;
		}
	}

	public void ResetCombo()
	{
		curCombo = 0;
	}
}
