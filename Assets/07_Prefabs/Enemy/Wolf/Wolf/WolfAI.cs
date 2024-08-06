using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class WolfAI : BasicAI
{
	[Header("IsWake")] [SerializeField] private bool _isWake;

	private const string NormalAttack = "normallAtt";
	

	
	public override void DieEvent(float delay = 0, float time = 3)
	{
		//self.anim.ResetStatus();
		StopExamine();
		WolfMoveModule _moveModule = self.move as WolfMoveModule;
		GetComponent<BoxCollider>().enabled = false;
		_moveModule.StopMove(); 
		base.DieEvent();
	}
    	
	public override void LookAt(Transform t)
	{
		Vector3 lookPos = t.position - transform.position;
		lookPos.y = transform.position.y;
		transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(lookPos), Time.deltaTime * 4);
	}
	
    public override void StartInvoke()
    {
	    head.connecteds.Clear();

	    Wolf_normalAttackModule _atkModule = self.atk as Wolf_normalAttackModule;

	    if (_isWake)
	    {
			IsNotStarted = true;
			self.anim.SetIdleState(true);
			self.life._dieEvent += () => { DieEvent(); };


			StunNode _ishaveStun = new StunNode(self, () =>
		    {
				//Debug.LogError(gameObject.name + " 일어남");//
		    });
		    Sequencer stunSeq = new Sequencer();

		    stunSeq.connecteds.Add(_ishaveStun);
		    
		    
		    #region 평타

		    Waiter _normalAtt = new Waiter(1.5f);
		    
		    IsInRange noramlRange = new IsInRange(self, player.transform, MoveRange, null, () =>
		    {

				_normalAtt.StartReady();
				_atkModule.SetAttackType(NormalAttack);
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
