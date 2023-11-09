using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearAI : MonoBehaviour
{
    Actor self;
	Actor player;

	Selecter head;

	bool stopped = false;

	private void Start()
	{
		player = GameManager.instance.pActor;

		self = GetComponent<Actor>();
		head = new Selecter();

		IsUnderBalance under70 = new IsUnderBalance(self, 0.7f);
		IsInRange inRangeSp = new IsInRange(self, player.transform, (self.atk as BearAttack).atkDist2, (player.move as PlayerMove).GetSneakDist,()=>
		{
			(self.atk as BearAttack).SetAttackType(AttackType.SpecialAttack);
		});
		Waiter waitSp = new Waiter(1);
		Attacker spAttack = new Attacker(self, ()=>
		{
			StopExamine();
		});
		Sequencer spAttacker = new Sequencer();
		spAttacker.connecteds.Add(under70);
		spAttacker.connecteds.Add(inRangeSp);
		spAttacker.connecteds.Add(waitSp);
		spAttacker.connecteds.Add(spAttack);


		IsInRange inRange2 = new IsInRange(self, player.transform, (self.atk as BearAttack).atkDist2, (player.move as PlayerMove).GetSneakDist, () =>
		{
			(self.atk as BearAttack).SetAttackType(AttackType.MouthAttack);
		});
		Waiter wait2 = new Waiter(10);
		Attacker atk2 = new Attacker(self, () =>
		{
			StopExamine();
		});
		Sequencer secondAttacker = new Sequencer();
		secondAttacker.connecteds.Add(inRange2);
		secondAttacker.connecteds.Add(wait2);
		secondAttacker.connecteds.Add(atk2);

		IsInRange inRange1 = new IsInRange(self, player.transform, self.atk.atkDist, (player.move as PlayerMove).GetSneakDist, () =>
		{
			(self.atk as BearAttack).SetAttackType(AttackType.HandAttack);
		});
		Waiter wait1 = new Waiter(2);
		Attacker atk1 = new Attacker(self, () =>
		{
			StopExamine();
		});
		Sequencer firstAttacker = new Sequencer();
		firstAttacker.connecteds.Add(inRange1);
		firstAttacker.connecteds.Add(wait1);
		firstAttacker.connecteds.Add(atk1);

		IsInRange inSight = new IsInRange(self, player.transform, self.sight.sightRange, (player.move as PlayerMove).GetSneakDist, ()=>
		{
			(self.move as BearMove).SetTarget(GameManager.instance.pActor.transform);
		});
		Mover move = new Mover(self);
		Sequencer chaser = new Sequencer();
		chaser.connecteds.Add(inSight);
		chaser.connecteds.Add(move);

		

		Inverter inv = new Inverter();
		inv.connected = inRange1;
		BearTargetResetter reset = new BearTargetResetter(self);
		Sequencer idler = new Sequencer();
		idler.connecteds.Add(inv);
		idler.connecteds.Add(reset);




		head.connecteds.Add(spAttacker);
		head.connecteds.Add(secondAttacker);
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

	public void StopExamine()
	{
		stopped = true;
		Debug.Log("STOP");
	}

	public void StartExamine()
	{
		stopped = false;
		Debug.Log("START");
	}

}
