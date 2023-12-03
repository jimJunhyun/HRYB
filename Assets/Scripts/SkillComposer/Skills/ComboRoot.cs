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
	float prevOperateSec;

	public override void Disoperate(Actor self)
	{
		ResetCombo();
		base.Disoperate(self);
	}

	public override void Operate(Actor self)
	{
		if(Time.time - prevOperateSec >= composeDel)
		{
			if (curCombo >= childs.Count)
			{
				ResetCombo();
				Debug.Log("콤보 최대, 초기화");
			}
			Debug.Log($"콤보 {curCombo + 1}/{childs.Count}");
			childs[curCombo].Operate(self);
			prevOperateSec = Time.time;
		}
	}

	public override void UpdateStatus()
	{
		if(curCombo	> resetThreshold && Time.time - prevComboSec >= resetSec && curCombo != 0)
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
		else if(curCombo < childs.Count - 1)
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
