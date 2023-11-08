using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    Actor self;

	//IComposable head;

	private void Awake()
	{
		self = GetComponent<Actor>();
	}

	private void Update()
	{
		//head.Compose();
	}
}
