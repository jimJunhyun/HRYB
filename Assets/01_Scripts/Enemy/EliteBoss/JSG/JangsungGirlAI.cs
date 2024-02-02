using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class JangsungGirlAI : AISetter
{
	public Actor _friend;

	private const string MumukNansa = "mumuk";
	private const string BarrierPatton = "Barrier";
	private const string RootPatton = "Root";
    protected override void StartInvoke()
    {
	    #region 보호막

	    Waiter waitBarrier = new Waiter(20f, true);
	    IsOutRange isBarrier = new IsOutRange(self, _friend.transform, BarrierRange, null, () =>
	    {
		    // Barrier 에니메이션 실행
		    // AttackMoudle에서 이름 셋팅
		    waitBarrier.StartReady();
	    });
	    Attacker BarrierGet = new Attacker(self, () =>
	    {
			waitBarrier.ResetReady();
			// AttackMoudle에서 이름 셋팅 << Attack에 유기하기
			
			StopExamine(); // LifeModule에서 풀어줘야됨 << 몇회 피격시니까
	    });

	    Sequencer BarrierPatton = new Sequencer();
	    
	    BarrierPatton.connecteds.Add(isBarrier);
	    BarrierPatton.connecteds.Add(waitBarrier);
	    BarrierPatton.connecteds.Add(BarrierGet);
	    

	    #endregion

	    #region 으아악 뿌리다

	    Waiter waitRootAttack = new Waiter(10f);
	    IsInRange isRootAttack = new IsInRange(self, player.transform, RootRange, null, () =>
	    {
		    // AttackMoudle에서 이름 셋팅 << Attack에 유기하기?
		    waitRootAttack.StartReady();
	    });

	    Attacker RootAttack = new Attacker(self, () =>
	    {
		    // 이름 셋팅 함 더하기
		    StopExamine(); // 패턴 끝나거나 (코루틴) or 캔슬시 코루틴 끊고 Start 해주기 
	    });

	    Sequencer RootPatton = new Sequencer();
	    RootPatton.connecteds.Add(isRootAttack);
	    RootPatton.connecteds.Add(waitRootAttack);
	    RootPatton.connecteds.Add(RootAttack);

	    #endregion

	    #region 묘목

	    Waiter waitSeedAttack = new Waiter(7f);
	    IsInRange isSeedAttack = new IsInRange(self, player.transform, SeedRange, null, () =>
	    {
		    // AttackMoudle에서 이름 셋팅 << Attack에 유기하기?
		    waitRootAttack.StartReady();
	    });

	    Attacker SeedAttack = new Attacker(self, () =>
	    {
			StopExamine();
	    });

	    Sequencer SeedPatton = new Sequencer();
	    SeedPatton.connecteds.Add(isRootAttack);
	    SeedPatton.connecteds.Add(waitBarrier);
	    SeedPatton.connecteds.Add(SeedAttack);

	    #endregion
	    
	    head.connecteds.Add(BarrierPatton);
	    head.connecteds.Add(RootPatton);
	    head.connecteds.Add(SeedPatton);
	    
	    StartExamine();
    }

    protected override void UpdateInvoke()
    {
	    
    }

    public float BarrierRange()
    {
	    return 25f;
    }

    public float RootRange()
    {
	    return 10f;
    }
    
    public float SeedRange()
    {
	    return 25f;
    }

}
