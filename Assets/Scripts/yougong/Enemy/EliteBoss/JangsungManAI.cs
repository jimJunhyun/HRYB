using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class JangsungManAI : AISetter
{
	public Actor _friend = new();
	
	private const string DownAttackHash = "DownAttack";
	
	private const string FallDownAttackHash = "FallDownAttack";
	
	private const string MoveAttackHash = "MoveAttack";
	
    protected override void StartInvoke()
    {
	    
	    JangsungManAttackModule _jsAttckModule = self.atk as JangsungManAttackModule;
	    JangSungMoveModule _jsMoveModule = self.move as JangSungMoveModule;


	    #region 조우

	    IsFirstTime firstFound = new IsFirstTime();
	    Waiter waitDuration = new Waiter((float)GameManager.instance.timeliner.duration, false, NodeStatus.Run);
	    IsInRange isAwake = new IsInRange(self, player.transform, self.sight.GetSightRange, _jsMoveModule.FindPlayer,
		    () =>
		    {
			    GameManager.instance.timeliner.Play();
			    GameManager.instance.pinp.DeactivateInput();
			    firstFound.Invalidate();
			    waitDuration.StartReady();
		    });
	    Inverter passed = new Inverter();
	    passed.connected = firstFound;
	    Selecter wakeUper = new Selecter();
	    wakeUper.connecteds.Add(passed);
	    wakeUper.connecteds.Add(isAwake);

	    #endregion

	    
	    
	    
	    IsTargetDead pairDead = new IsTargetDead(_friend);
	    IsFirstTime _Powered = new IsFirstTime();
	    Waiter powerWait = new Waiter(2f, true, NodeStatus.Fail, false, ()=>
	    {
		    // 여기서 스텟조정
		    _jsMoveModule.PowerUp();
	    });

	    #region 점프 공격
	    Waiter waitJump = new Waiter(5f);
	    IsInRange isJumpAtk = new IsInRange(self, player.transform, _jsAttckModule.JumpDist, null, () =>
	    {
		    _jsAttckModule.SetAttackType(FallDownAttackHash);
		    waitJump.StartReady();
	    });
	    Attacker JumpAttack = new Attacker(self, () =>
	    {
		    _jsMoveModule.LookAt(player.transform);
		    _jsMoveModule.FallDownAttack();
		    _jsMoveModule.ResetDest();
		    
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
		    _jsAttckModule.SetAttackType(MoveAttackHash);
		    waitJump.StartReady();
	    });
	    
	    Inverter moveSerch = new Inverter();
	    moveSerch.connected = isMoveAttack;
	    
	    Attacker MoveAttack = new Attacker(self, () =>
	    {

		    _jsMoveModule.LookAt(player.transform);
		    _jsMoveModule.NormalMoveAttack();
		    _jsMoveModule.ResetDest();
		    
		    StopExamine();
	    });
	    
	    Sequencer MoveATK = new Sequencer();
	    
	    MoveATK.connecteds.Add(waitMove);
	    MoveATK.connecteds.Add(moveSerch);
	    MoveATK.connecteds.Add(MoveAttack);
	    #endregion

	    #region WaitDown
	    Waiter DownWait = new Waiter(2f);
	    IsInRange isRangeDown = new IsInRange(self, player.transform, _jsAttckModule.DownAttack, null, () =>
	    {
		    
		    _jsAttckModule.SetAttackType(DownAttackHash);
		    waitJump.StartReady();
	    });

	    Attacker DownAttack = new Attacker(self, () =>
	    {
		    _jsMoveModule.LookAt(player.transform);
		    _jsMoveModule.ResetDest();
		    StopExamine();
	    });

	    Sequencer DownATK = new Sequencer();
	    DownATK.connecteds.Add(DownWait);
	    DownATK.connecteds.Add(isRangeDown);
	    DownATK.connecteds.Add(DownAttack);


	    #endregion
		head.connecteds.Add(MoveATK);
		head.connecteds.Add(JumpATK);
		head.connecteds.Add(DownATK);
		
    }

    protected override void UpdateInvoke()
    {
	    
    }
    
    public override void StartExamine()
    {
	    stopped = false;
    }
}
