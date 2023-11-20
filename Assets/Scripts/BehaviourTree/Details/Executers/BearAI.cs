using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearAI : MonoBehaviour
{
    Actor self;
	Actor player;

	Selecter head;

	public Transform targetPo;

	bool stopped = false;

	private void Start()
	{
		player = GameManager.instance.pActor;

		self = GetComponent<Actor>();
		head = new Selecter();

		IsFirstTime firstFound = new IsFirstTime();
		IsInRange inWakeUp = new IsInRange(self, player.transform, self.sight.GetSightRange, (player.move as PlayerMove).GetSneakDist, () =>
		{
			(self.move as BearMove).SetTarget(targetPo);
			(self.anim as BearAnim).ResetSleepMode();
			firstFound.Invalidate();
			Debug.Log("FOUDNEN");
		});
		Inverter passed = new Inverter();
		passed.connected = firstFound;
		Selecter wakeUper = new Selecter();
		wakeUper.connecteds.Add(passed);
		wakeUper.connecteds.Add(inWakeUp);

		IsFirstTime firstArrive = new IsFirstTime();
		IsInRange arrival = new IsInRange(self, targetPo, ()=>1f, null, ()=>
		{
			firstArrive.Invalidate();
		});
		Mover escape = new Mover(self);
		Sequencer escaper = new Sequencer();
		Inverter notArrive = new Inverter();
		notArrive.connected = arrival;

		escaper.connecteds.Add(wakeUper);
		escaper.connecteds.Add(firstArrive);
		escaper.connecteds.Add(notArrive);
		escaper.connecteds.Add(escape);


		IsFirstTime once = new IsFirstTime();
		IsUnderBalance under70 = new IsUnderBalance(self, 0.7f);
		IsInRange inRangeSp = new IsInRange(self, player.transform, (self.atk as BearAttack).GetDist3, null,()=>
		{
			(self.atk as BearAttack).SetAttackType(AttackType.SpecialAttack);
		});
		Waiter waitSp = new Waiter(1);
		Attacker spAttack = new Attacker(self, ()=>
		{
			(self.move as BearMove).LookAt(player.transform);
			(self.move as BearMove).ResetDest();
			once.Invalidate();
			StopExamine();
			
		});
		Sequencer spAttacker = new Sequencer();
		spAttacker.connecteds.Add(once);
		spAttacker.connecteds.Add(under70);
		spAttacker.connecteds.Add(inRangeSp);
		spAttacker.connecteds.Add(waitSp);
		spAttacker.connecteds.Add(spAttack);


		IsInRange inRange2 = new IsInRange(self, player.transform, (self.atk as BearAttack).GetDist2, null, () =>
		{
			(self.atk as BearAttack).SetAttackType(AttackType.MouthAttack);
			(self.atk as BearAttack).SetTarget(player);
		});
		Waiter wait2 = new Waiter(10);
		Attacker atk2 = new Attacker(self, () =>
		{
			(self.move as BearMove).LookAt(player.transform);
			(self.move as BearMove).ResetDest();
			StopExamine();
		});
		Sequencer secondAttacker = new Sequencer();
		secondAttacker.connecteds.Add(inRange2);
		secondAttacker.connecteds.Add(wait2);
		secondAttacker.connecteds.Add(atk2);

		IsInRange inRange1 = new IsInRange(self, player.transform, self.atk.GetDist, null, () =>
		{
			(self.atk as BearAttack).SetAttackType(AttackType.HandAttack);
		});
		Waiter wait1 = new Waiter(2);
		Attacker atk1 = new Attacker(self, () =>
		{
			(self.move as BearMove).LookAt(player.transform);
			(self.move as BearMove).ResetDest();
			StopExamine();
		});
		Sequencer firstAttacker = new Sequencer();
		firstAttacker.connecteds.Add(inRange1);
		firstAttacker.connecteds.Add(wait1);
		firstAttacker.connecteds.Add(atk1);

		IsInRange inSight = new IsInRange(self, player.transform, self.sight.GetSightRange, (player.move as PlayerMove).GetSneakDist, ()=>
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


		head.connecteds.Add(escaper);
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
	}

	public void StartExamine()
	{
		stopped = false;
	}

}
