using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearAI : MonoBehaviour
{
    Actor self;

	Selecter head;

	private void Awake()
	{
		self = GetComponent<Actor>();
		head = new Selecter();

		IsInRange inSight = new IsInRange(self, GameManager.instance.player.transform, self.sight.sightRange);
		Mover move = new Mover(self);
		Sequencer chases = new Sequencer();
		chases.connecteds.Add(inSight);
		chases.connecteds.Add(move);

		IsInRange inRange1 = new IsInRange(self, GameManager.instance.player.transform, self.atk.atkDist, ()=>
		{
			(self.atk as BearAttack).SetAttackType(AttackType.HandAttack);
		});
		Waiter wait1 = new Waiter(2);
		Attacker atk1 = new Attacker(self);
		Sequencer firstAttacker = new Sequencer();
		firstAttacker.connecteds.Add(wait1);
		firstAttacker.connecteds.Add(inRange1);
		firstAttacker.connecteds.Add(atk1);

		//2공격만들기

		//3공격만들기


		//3공격
		//2공격
		head.connecteds.Add(atk1);
		head.connecteds.Add(chases);
	}

	private void Update()
	{
		head.Examine();
	}
}
