using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Infos/SpeedUp")]
public class SpeedUp : Leaf
{
	public float mult;
	public float duration;


	internal override void MyOperation(Actor self)
	{
		self.move.moveModuleStat.HandleSpeed(-level, ModuleController.SpeedMode.Slow);
		Debug.Log("이속증가");
		GameManager.instance.StartCoroutine(DelDisoperate(self));
	}
	internal override void MyDisoperation(Actor self)
	{
		self.move.moveModuleStat.HandleSpeed(level, ModuleController.SpeedMode.Slow);
	}

	
	public override void UpdateStatus()
	{
		throw new System.NotImplementedException();
	}
	
	IEnumerator DelDisoperate(Actor self)
	{
		yield return new WaitForSeconds(duration);
		MyDisoperation(self);
	}
}
