using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    Actor self;

	Selecter head;

	private void Awake()
	{
		self = GetComponent<Actor>();
		head = new Selecter();
		IsInRange inRange = new IsInRange(self, GameManager.instance.player.transform, self.sight.sightRange);
		Mover move = new Mover(self);
		Sequencer seq = new Sequencer();
		seq.connecteds.Add(inRange);
		seq.connecteds.Add(move);
		head.connecteds.Add(seq);

	}

	private void Update()
	{
		head.Examine();
	}
}
