using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Infos/SpeedUp")]
public class SpeedUp : Leaf
{
	public float mult;
	public float duration;


	protected override void MyOperation(Actor self)
	{
		self.move.speedMod += 0.5f;
		Debug.Log("이속증가");
	}
	protected override void MyDisoperation(Actor self)
	{
		self.move.speedMod -= 0.5f;
	}
}
