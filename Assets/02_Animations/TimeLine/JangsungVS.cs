using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JangsungVS : MonoBehaviour
{
	[SerializeField] Transform tls;
	[SerializeField] Transform tls2;

    public void PlayerOnMove()
	{
		GameManager.instance.player.GetComponent<PlayerMove>().PlayerTeleport(tls.transform.position);

	}

	public void PlayerOnSpawn()
	{
		GameManager.instance.DisableCtrl();
		GameManager.instance.player.GetComponent<PlayerMove>().PlayerTeleport(new Vector3(917.5f, 18f,222f));
	}

	public void SceneLoadSpawn()
	{
		SceneManager.LoadScene("Title");
	}
}
