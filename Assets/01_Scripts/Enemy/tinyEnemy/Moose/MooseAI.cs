using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MooseAI : BasicAI
{
	
	[Header("공격 시작 범위")] [SerializeField] public float _attackRange = 4f;



	bool _isFind = false;
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
		    IsInRange noramlRange = new IsInRange(self, player.transform, MoveRange, null, () =>
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



			head.connecteds.Add(stunSeq);
			head.connecteds.Add(normalATK);

			base.StartInvoke();


		    //_moveModule.StopMove();
	    }
	    else
	    {
		    self.anim.SetBoolModify("Sleep", true);
	    }
	    
    }

}
