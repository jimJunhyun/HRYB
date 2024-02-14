using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class JangsungGirlAI : AISetter
{
	public Actor _friend;

	private const string _MumukNansa = "mumuk";
	private const string _BarrierPattonname = "Barrier";
	private const string _RootPatton = "Root";
    protected override void StartInvoke()
    {
		JangsungGirlAttack _att = self.atk as JangsungGirlAttack;
		JangsungGirlLifeModule _life = self.life as JangsungGirlLifeModule;


	    #region 보호막

	    Waiter waitBarrier = new Waiter(0.5f, true);
	    IsOutRange isBarrier = new IsOutRange(self, _friend.transform, BarrierRange, null, () =>
	    {
			// Barrier 에니메이션 실행
			// AttackMoudle에서 이름 셋팅
			_att.SetAttackType(_BarrierPattonname);
		    waitBarrier.StartReady();
	    });
	    Attacker BarrierGet = new Attacker(self, () =>
	    {
			waitBarrier.ResetReady();
			// AttackMoudle에서 이름 셋팅 << Attack에 유기하기
			_life.BarrierON(5);
			Debug.LogError("보호막패턴");
			self.anim.SetAttackTrigger();
			self.anim.Animators.SetBool(_BarrierPattonname, true);
			StopExamine(); // LifeModule에서 풀어줘야됨 << 몇회 피격시니까
	    });

	    Sequencer BarrierPatton = new Sequencer();
	    
	    BarrierPatton.connecteds.Add(isBarrier);
	    BarrierPatton.connecteds.Add(waitBarrier);
	    BarrierPatton.connecteds.Add(BarrierGet);
	    

	    #endregion

	    #region 으아악 뿌리다

	    Waiter waitRootAttack = new Waiter(20.0f);
	    IsInRange isRootAttack = new IsInRange(self, player.transform, RootRange, null, () =>
	    {
			// AttackMoudle에서 이름 셋팅 << Attack에 유기하기?
			_att.SetAttackType(_RootPatton);
			waitRootAttack.StartReady();
	    });

	    Attacker RootAttack = new Attacker(self, () =>
	    {
			waitRootAttack.ResetReady();
			_att.SetAttackType(_RootPatton);
			// 이름 셋팅 함 더하기
			self.anim.Animators.SetBool(_RootPatton, true);
			self.anim.SetAttackTrigger();
			Debug.LogError("땅바닥");
			StopExamine(); // 패턴 끝나거나 (코루틴) or 캔슬시 코루틴 끊고 Start 해주기 
	    });

	    Sequencer RootPatton = new Sequencer();
	    RootPatton.connecteds.Add(isRootAttack);
	    RootPatton.connecteds.Add(waitRootAttack);
	    RootPatton.connecteds.Add(RootAttack);

	    #endregion

	    #region 묘목

	    Waiter waitSeedAttack = new Waiter(12.0f);
	    IsInRange isSeedAttack = new IsInRange(self, player.transform, SeedRange, null, () =>
	    {
			// AttackMoudle에서 이름 셋팅 << Attack에 유기하기?
			_att.SetAttackType(_MumukNansa);
			waitSeedAttack.StartReady();
	    });
	    Attacker SeedAttack = new Attacker(self, () =>
	    {
			waitSeedAttack.ResetReady();
			_att.SetAttackType(_MumukNansa);
			self.anim.Animators.SetBool(_MumukNansa, true);
			self.anim.SetAttackTrigger();
			Debug.LogError("뮤뮥");
			StopExamine();
	    });

	    Sequencer SeedPatton = new Sequencer();
	    SeedPatton.connecteds.Add(isSeedAttack);
	    SeedPatton.connecteds.Add(waitSeedAttack);
	    SeedPatton.connecteds.Add(SeedAttack);

	    #endregion
	    
	    head.connecteds.Add(BarrierPatton);
	    head.connecteds.Add(RootPatton);
	    head.connecteds.Add(SeedPatton);
		Debug.LogError("실행중");
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
