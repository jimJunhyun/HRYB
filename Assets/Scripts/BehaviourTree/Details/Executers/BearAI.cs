using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearAI : MonoBehaviour
{
    Actor self;

	Selecter head;

	bool stopped = false;

	private void Start()
	{
		self = GetComponent<Actor>();
		head = new Selecter();

		IsInRange inSight = new IsInRange(self, GameManager.instance.player.transform, self.sight.sightRange, ()=>
		{
			(self.move as BearMove).SetTarget(GameManager.instance.pActor.transform);
		});
		Mover move = new Mover(self);
		Sequencer chaser = new Sequencer();
		chaser.connecteds.Add(inSight);
		chaser.connecteds.Add(move);

		IsInRange inRange1 = new IsInRange(self, GameManager.instance.player.transform, self.atk.atkDist, ()=>
		{
			(self.atk as BearAttack).SetAttackType(AttackType.HandAttack);
		});
		Waiter wait1 = new Waiter(2);
		Attacker atk1 = new Attacker(self,  ()=>
		{ 
			wait1.ResetReady();
			StartCoroutine(DelayExamine(1));
		});
		Sequencer firstAttacker = new Sequencer();
		firstAttacker.connecteds.Add(wait1);
		firstAttacker.connecteds.Add(inRange1);
		firstAttacker.connecteds.Add(atk1);

		Inverter inv = new Inverter();
		inv.connected = inRange1;
		BearTargetResetter reset = new BearTargetResetter(self);
		Sequencer idler = new Sequencer();
		idler.connecteds.Add(inv);
		idler.connecteds.Add(reset);

		//2공격만들기

		//3공격만들기


		//3공격
		//2공격
		head.connecteds.Add(firstAttacker);
		head.connecteds.Add(chaser);
		head.connecteds.Add(idler);
	}

	private void Update()
	{
		if (!stopped)
		{
			head.Examine();
		}
	}

	IEnumerator DelayExamine(float t)
	{
		stopped = true;
		yield return new WaitForSeconds(t);
		stopped = false;
	}
}
