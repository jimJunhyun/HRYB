using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MooseAI : AISetter
{
	[Header("IsWake")] [SerializeField] private bool _isWake;
	
	
	[Header("공격 시작 범위")] [SerializeField] public float _attackRange = 4f;

	[Header("탐색 범위")] [SerializeField] public float _sectionRange = 10f;
	[Header("초기화 범위")] [SerializeField] public float _section2Range = 15f;

	bool _isFind = false;
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

	public override void DieEvent(float delay = 0, float time = 3)
	{
		//self.anim.ResetStatus();
		StopExamine();
		MooseMoveModule _moveModule = self.move as MooseMoveModule;
		GetComponent<BoxCollider>().enabled = false;
		_moveModule.StopMove();
		base.DieEvent();
	}
    	
	
    public override void StartInvoke()
    {
	    head.connecteds.Clear();

	    Moose_normalAttackModule _atkModule = self.atk as Moose_normalAttackModule;
	    MooseMoveModule _moveModule = self.move as MooseMoveModule;
	    
	    if (_isWake)
	    {
			IsNotStarted = true;
			self.anim.SetIdleState(true);
			self.life._hitEvent += _moveModule.StopMove;
			self.life._dieEvent += () => { DieEvent(); };

			StunNode _ishaveStun = new StunNode(self, () =>
		    {
			    //Debug.LogError(gameObject.name + " 일어남");
		    });
		    Sequencer stunSeq = new Sequencer();

		    stunSeq.connecteds.Add(_ishaveStun);

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


			#region Noramled
			IsInRange DetectedRange = new IsInRange(self, player.transform, this.OutSectionRanged, null, () =>
			{
				Vector3 dir = (self.transform.position - player.transform.position);
				if (SectionRanged() * SectionRanged() < dir.sqrMagnitude)
					_isFind = true;

				if(_isFind)
				_moveModule.SetTarget(player.transform);
			});	
			Mover move = new Mover(self);

			Sequencer Moved = new Sequencer();
			Moved.connecteds.Add(DetectedRange);
			Moved.connecteds.Add(move);

			IsOutRange LongaRange = new IsOutRange(self, player.transform, OutSectionRanged, null, () =>
			{
				_moveModule.StopMove();
				_isFind = false;

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
			#endregion


			head.connecteds.Add(stunSeq);
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
		if ((self.life.IsFirstHit == true || Vector3.Distance(player.transform.position, transform.position) < 7) && _isWake == false)
		{
			_isWake = true;
			self.anim.SetBoolModify("Sleep", false);
			StartInvoke();
		}

		if (self.AI.StopState)
			return;

		if (self.life.isDead == false && _isWake && self.life.isDead == false && self.anim.Animators.GetBool("Stun") == false)
	    {
		    LookAt(player.transform);
		    
	    }

	    transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
	    

    }
}
