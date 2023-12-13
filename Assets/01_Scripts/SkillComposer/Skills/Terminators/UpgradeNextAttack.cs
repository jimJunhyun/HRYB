using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Infos2/UpgradeNextAttack")]
public class UpgradeNextAttack : Leaf
{
	public YinYang upgradeDamage;
	public List<StatusEffectApplyData> upgradeStatEff;

	public override void UpdateStatus()
	{
		
	}

	internal override void MyDisoperation(Actor self)
	{
		
	}

	internal override void MyOperation(Actor self)
	{
		//not established.
	}
}
