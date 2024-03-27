using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class JangsungManAI : AISetter
{
	public Actor _friend;
	
	private const string DownAttackHash = "DownAttack";
	
	private const string FallDownAttackHash = "FallDownAttack";
	
	private const string MoveAttackHash = "MoveAttack";
	

	public void DieEvent()
	{
		self.anim.ResetStatus();
		StopExamine();
	}

    protected override void StartInvoke()
    {
		//GetComponent<BossHPBar>().Init(self);


		self.life._dieEvent = DieEvent;


		JangsungManAttackModule _jsAttckModule = self.atk as JangsungManAttackModule;
	    JangSungMoveModule _jsMoveModule = self.move as JangSungMoveModule;
	    _jsAttckModule.SetTarget(player);
	    _jsMoveModule.SetTarget(player);
	    #region 조우

	    //IsFirstTime firstFound = new IsFirstTime();
	    //Waiter waitDuration = new Waiter((float)GameManager.instance.timeliner.duration, false, NodeStatus.Run);
	    //IsInRange isAwake = new IsInRange(self, player.transform, self.sight.GetSightRange, _jsMoveModule.FindPlayer,
		//    () =>
		//    {
		//	    GameManager.instance.timeliner.Play();
		//	    GameManager.instance.pinp.DeactivateInput();
		//	    firstFound.Invalidate();
		//	    waitDuration.StartReady();
		//    });
	    //Inverter passed = new Inverter();
	    //passed.connected = firstFound;
	    //Selecter wakeUper = new Selecter();
	    //wakeUper.connecteds.Add(passed);
	    //wakeUper.connecteds.Add(isAwake);
//
	    #endregion

	    
	    
	    
	    //IsTargetDead pairDead = new IsTargetDead(_friend);
	    //IsFirstTime _Powered = new IsFirstTime();
	    //Waiter powerWait = new Waiter(2f, true, NodeStatus.Fail, false, ()=>
	    //{
		//    // 여기서 스텟조정
		//    _jsMoveModule.PowerUp();
	    //});

	    #region 점프 공격
	    Waiter waitJump = new Waiter(10f);
	    IsOutRange isJumpAtk = new IsOutRange(self, player.transform, _jsAttckModule.JumpDist, null, () =>
	    {
		    _jsAttckModule.SetAttackType(FallDownAttackHash);
		    _jsAttckModule.SetTarget(player);
		    waitJump.StartReady();
	    });
	    Attacker JumpAttack = new Attacker(self, () =>
	    {
		    waitJump.ResetReady();
		    _jsMoveModule.SetTarget(player);
		    _jsMoveModule.LookAt(player.transform);
		    
		    //_jsMoveModule.ResetDest();
		    
		    StopExamine();
	    });
	    
	    //Inverter notArrive3 = new Inverter();
	    //notArrive3.connected = pairDead;
	    
	    Sequencer JumpATK = new Sequencer();
	    
	    JumpATK.connecteds.Add(isJumpAtk);
	    JumpATK.connecteds.Add(waitJump);
	    JumpATK.connecteds.Add(JumpAttack);


	    #endregion

	    #region 이동 공격
	    Waiter waitMove = new Waiter(0.5f);
	    
	    IsInRange isMoveAttack = new IsInRange(self, player.transform, _jsAttckModule.MoveAttack, null, () =>
	    {
		    //Debug.LogWarning($"이동 : 거리재기");

		    _jsAttckModule.SetAttackType(MoveAttackHash);
		    _jsAttckModule.SetTarget(player);
		    waitMove.StartReady();
	    });
	    
	    //Inverter moveSerch = new Inverter();
	    //moveSerch.connected = isMoveAttack;
	    
	    Attacker MoveAttack = new Attacker(self, () =>
	    {		    
		    //Debug.LogWarning($"이동 : 공격시작");

		    waitMove.ResetReady();
		    _jsMoveModule.SetTarget(player);
		    _jsMoveModule.LookAt(player.transform);
		    
		    
		    
		    StopExamine();
	    });
	    
	    Sequencer MoveATK = new Sequencer();
	    
	    MoveATK.connecteds.Add(isMoveAttack);
	    MoveATK.connecteds.Add(waitMove);
	    MoveATK.connecteds.Add(MoveAttack);
	    #endregion

	    #region WaitDown
	    Waiter DownWait = new Waiter(9f);
	    IsInRange isRangeDown = new IsInRange(self, player.transform, _jsAttckModule.DownAttack, null, () =>
	    {
		    _jsAttckModule.SetTarget(player);
		    _jsAttckModule.SetAttackType(DownAttackHash);
		    DownWait.StartReady();
	    });

	    Attacker DownAttack = new Attacker(self, () =>
	    {
		    DownWait.ResetReady();
		    _jsMoveModule.SetTarget(player);
		    _jsMoveModule.LookAt(player.transform);
		    _jsMoveModule.ResetDest();
		    StopExamine();
	    });

	    Sequencer DownATK = new Sequencer();
	    DownATK.connecteds.Add(isRangeDown);
	    DownATK.connecteds.Add(DownWait);
	    DownATK.connecteds.Add(DownAttack);


	    #endregion
	    
	    

		head.connecteds.Add(JumpATK);
		head.connecteds.Add(DownATK);
		head.connecteds.Add(MoveATK);

		StartExamine();
		
    }

    protected override void UpdateInvoke()
    {
	    
    }
    
}
