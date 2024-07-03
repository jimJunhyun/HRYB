using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTimeLineBind : MonoBehaviour
{
	[SerializeField] Transform tls;
	public void PlayerOnMove()
	{
		PlayerDisable();
		GameManager.instance.player.GetComponent<PlayerMove>().PlayerTeleport(tls.transform.position);

	}

	public void PlayerDisable()
	{
		GameManager.instance.DisableCtrl();
	}

	public void PlayerActive()
	{
		GameManager.instance.EnableCtrl();
		GameManager.instance.PlayTimeline("StartTimeline");
	}
}
