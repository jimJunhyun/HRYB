using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearAI : AISetter
{
	public Transform targetPo1;
	public Transform targetPo2;
	public Transform targetPo3;
	protected override void StartInvoke()
	{

		IsFirstTime firstFound = new IsFirstTime();
		Waiter waitDuration = new Waiter((float)GameManager.instance.timeliner.duration, false, false, NodeStatus.Run);
		IsInRange inWakeUp = new IsInRange(self, player.transform, self.sight.GetSightRange, (player.move as PlayerMove).GetSneakDist, ()=>
		{
			(self.anim as BearAnim).ResetSleepMode();
			GameManager.instance.timeliner.Play();
			GameManager.instance.pinp.DeactivateInput();
			firstFound.Invalidate();
			waitDuration.StartReady();
		});

		Inverter passed = new Inverter();
		passed.connected = firstFound;
		Selecter wakeUper = new Selecter();
		wakeUper.connecteds.Add(passed);
		wakeUper.connecteds.Add(inWakeUp);

		IsFirstTime passedWait = new IsFirstTime();
		IsInRange onWakeUp = new IsInRange(self, self.transform, self.sight.GetSightRange, null, () =>
		{
			(self.move as BearMove).SetTarget(targetPo1);
			GameManager.instance.pinp.ActivateInput();
			passedWait.Invalidate();
			Debug.Log("CINEMA END");
		});
		Inverter waitPass = new Inverter();
		waitPass.connected = passedWait;

		Sequencer waitActivate =  new Sequencer();
		waitActivate.connecteds.Add(waitDuration);
		waitActivate.connecteds.Add(onWakeUp);
		
		Selecter waitActivater = new Selecter();
		waitActivater.connecteds.Add(waitPass);
		waitActivater.connecteds.Add(waitActivate);

		IsFirstTime firstArrived = new IsFirstTime();
		IsInRange arrival1 = new IsInRange(self, targetPo1, ()=>1f, null, ()=>
		{
			firstArrived.Invalidate();
			(self.move as BearMove).SetTarget(targetPo2);
			Debug.Log("!@#!#");
		});
		Mover escape = new Mover(self);
		Inverter hasArrived1 = new Inverter();
		hasArrived1.connected = firstArrived;

		Selecter arrived1 = new Selecter();
		arrived1.connecteds.Add(arrival1);
		arrived1.connecteds.Add(hasArrived1);

		Inverter notArrived1 = new Inverter();
		notArrived1.connected = arrived1;

		Sequencer escaper = new Sequencer();


		escaper.connecteds.Add(wakeUper);
		escaper.connecteds.Add(waitActivater);
		escaper.connecteds.Add(notArrived1); 
		escaper.connecteds.Add(escape); 


		IsFirstTime secondArrive = new IsFirstTime();
		IsInRange arrival2 = new IsInRange(self, targetPo2, () => 1f, null, () =>
		{
			secondArrive.Invalidate();
			(self.move as BearMove).SetTarget(targetPo3);
		});
		Sequencer escaper2 = new Sequencer();
		Inverter notArrive2 = new Inverter();
		notArrive2.connected = arrival2;

		escaper2.connecteds.Add(secondArrive);
		escaper2.connecteds.Add(notArrive2);
		escaper2.connecteds.Add(escape);

		IsFirstTime thirdArrive = new IsFirstTime();
		IsInRange arrival3 = new IsInRange(self, targetPo3, () => 1f, null, () =>
		{
			thirdArrive.Invalidate();
			(self.move as BearMove).SetTarget(null);
		});
		Sequencer escaper3 = new Sequencer();
		Inverter notArrive3 = new Inverter();
		notArrive3.connected = arrival3;

		escaper3.connecteds.Add(thirdArrive);
		escaper3.connecteds.Add(notArrive3);
		escaper3.connecteds.Add(escape);


		IsFirstTime once = new IsFirstTime();
		IsUnderBalance under70 = new IsUnderBalance(self, 0.7f);
		Waiter waitSp = new Waiter(1);
		IsInRange inRangeSp = new IsInRange(self, player.transform, (self.atk as BearAttack).GetDist3, null,()=>
		{
			(self.atk as BearAttack).SetAttackType(AttackType.SpecialAttack);
			waitSp.StartReady();
		});
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
		Waiter wait2 = new Waiter(10, false, true, NodeStatus.Fail, true);
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

		Waiter wait1 = new Waiter(2);
		IsInRange inRange1 = new IsInRange(self, player.transform, self.atk.GetDist, null, () =>
		{
			(self.atk as BearAttack).SetAttackType(AttackType.HandAttack);
			wait1.StartReady();
		});
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
			GameManager.instance.sManager.ProceedTo(GameState.Fight);
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
		head.connecteds.Add(escaper2);
		head.connecteds.Add(escaper3);
		head.connecteds.Add(spAttacker);
		head.connecteds.Add(secondAttacker);
		head.connecteds.Add(firstAttacker);
		head.connecteds.Add(chaser);
		head.connecteds.Add(idler);
	}

	protected override void UpdateInvoke()
	{
		
	}
}
