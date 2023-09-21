using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject player;
	public PlayerInput pinp;
	public ItemManager itemManager;

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
		pinp = player.GetComponent<PlayerInput>();
		itemManager = GameObject.Find("ItemManager").GetComponent<ItemManager>();
	}
}
