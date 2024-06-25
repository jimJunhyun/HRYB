using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitState : StateMachineBehaviour
{

	/// 임시 방편임;;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateEnter(animator, stateInfo, layerIndex);
		try
		{
			animator.GetComponent<Actor>().life.PlayhitAgain();
		}
		catch
		{
			GameManager.instance.pActor.life.PlayhitAgain();
		}
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		base.OnStateExit(animator, stateInfo, layerIndex);
		try
		{
			animator.GetComponent<Actor>().life.PlayWakeAgain();
		}
		catch
		{
			GameManager.instance.pActor.life.PlayWakeAgain();
		}
	}

}
