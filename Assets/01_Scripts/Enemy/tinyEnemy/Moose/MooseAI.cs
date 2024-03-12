using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MooseAI : AISetter
{
	[Header("IsWake")] [SerializeField] private bool _isWake;
	
	
	[Header("공격 시작 범위")] [SerializeField] public float _attackRange = 4f;

	[Header("탐색 범위")] [SerializeField] public float _sectionRange = 10f;
	[Header("초기화 범위")] [SerializeField] public float _section2Range = 15f;
	
	public float Attackrange()
	{
		return _attackRange;
	}
     	
	public float SectionRanged()
	{
		return _sectionRange;
	}
  	
	public float OutSectionRanged()
	{
		return _section2Range;
	}

	public void DieEvent()
	{
		//self.anim.ResetStatus();
		StopExamine();
		MooseMoveModule _moveModule = self.move as MooseMoveModule;
		GetComponent<BoxCollider>().isTrigger = true;
		_moveModule.StopMove();
	}
    	
	public void LookAt(Transform t)
	{
		Vector3 lookPos = t.position - transform.position;
		lookPos.y = transform.position.y;
		transform.rotation = Quaternion.LookRotation(lookPos);
	}
	
    protected override void StartInvoke()
    {
	    head.connecteds.Clear();
	    self.life._dieEvent = DieEvent;
	    Moose_normalAttackModule _atkModule = self.atk as Moose_normalAttackModule;
	    MooseMoveModule _moveModule = self.move as MooseMoveModule;
	    
	    if (_isWake)
	    {
		    self.anim.SetIdleState(true);




		    #region 평타

		    Waiter _normalAtt = new Waiter(5f);
		    IsInRange noramlRange = new IsInRange(self, player.transform, Attackrange, null, () =>
		    {

			    _normalAtt.StartReady();
			    //_atkModule.SetAttackType(NormalAttack);
			    _moveModule.StopMove();
			    

		    });
		    Attacker normalAttack = new Attacker(self, () =>
		    {
			    _normalAtt.ResetReady();

			    StopExamine();
		    });

		    Sequencer normalATK = new Sequencer();

		    normalATK.connecteds.Add(noramlRange);
		    normalATK.connecteds.Add(_normalAtt);
		    normalATK.connecteds.Add(normalAttack);

		    #endregion

		    IsInRange SectionRange = new IsInRange(self, player.transform, this.SectionRanged, null, () =>
		    {

			    _moveModule.SetTarget(player.transform);


		    });
		    Mover move = new Mover(self);

		    Sequencer Moved = new Sequencer();
		    Moved.connecteds.Add(SectionRange);
		    Moved.connecteds.Add(move);

		    IsOutRange LongaRange = new IsOutRange(self, player.transform, OutSectionRanged, null, () =>
		    {
			    _moveModule.StopMove();
		    });
		    IsInRange Idler = new IsInRange(self, player.transform, Attackrange, null, () =>
		    {
			    _moveModule.StopMove();
		    });

		    Idler idles = new Idler(self);

		    Sequencer Faridler = new Sequencer();
		    Faridler.connecteds.Add(LongaRange);
		    Faridler.connecteds.Add(idles);

		    Sequencer ShowIdler = new Sequencer();

		    ShowIdler.connecteds.Add(Idler);
		    ShowIdler.connecteds.Add(idles);

		    head.connecteds.Add(normalATK);
		    head.connecteds.Add(ShowIdler);
		    head.connecteds.Add(Moved);
		    head.connecteds.Add(Faridler);


		    //_moveModule.StopMove();
	    }
	    else
	    {
		    self.anim.SetBoolModify("Sleep", true);
	    }
	    
    }

    protected override void UpdateInvoke()
    {
	    if(self.life.isDead ==false && _isWake)
		    LookAt(player.transform);

	    if ((self.life.IsFirstHit == true || Vector3.Distance(player.transform.position, transform.position) < 7) && _isWake == false)
	    {
		    _isWake = true;
		    self.anim.SetBoolModify("Sleep", false);
		    StartInvoke();
	    }
    }
}
