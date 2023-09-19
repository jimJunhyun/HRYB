using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject player;

	public bool lockMouse;

	private void Awake()
	{
		instance = this;

		if (lockMouse)
		{
			Cursor.lockState = CursorLockMode.Locked;
			//Cursor.visible = false;
		}

		player = GameObject.Find("Player");
	}
}
