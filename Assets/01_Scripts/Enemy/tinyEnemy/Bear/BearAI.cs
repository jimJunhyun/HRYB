using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearAI : AISetter
{
	[Header("IsWake")][SerializeField] private bool _isWake;


	[Header("공격 시작 범위")][SerializeField] public float _attackRange = 4f;
	[Header("특수 공격1 시작 범위")][SerializeField] public float _exAttackRange = 12f;

	
	
	[Header("탐색 범위")][SerializeField] public float _sectionRange = 10f;
	[Header("초기화 범위")][SerializeField] public float _section2Range = 15f;

	const string NormalAttack = "Normal";
	const string EXAttack = "EX";
	const string EX2Attack = "EX2";
	public float Attackrange()
	{
		return _attackRange;
	}
	
	public float EX1Attackrange()
	{
		return _exAttackRange;
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


	protected override void StartInvoke()
	{
		head.connecteds.Clear();
		self.life._dieEvent = DieEvent;
		Bear_AttackModule _atkModule = self.atk as Bear_AttackModule;
		BearMoveModule _moveModule = self.move as BearMoveModule;

		if (_isWake)
		{
			self.anim.SetIdleState(true);

			StunNode _ishaveStun = new StunNode(self, () =>
			{
				Debug.LogError(gameObject.name + " 일어남");
			});
			Sequencer stunSeq = new Sequencer();

			stunSeq.connecteds.Add(_ishaveStun);


			#region 기본공격

			Waiter _normalAtt = new Waiter(5f);
			IsInRange noramlRange = new IsInRange(self, player.transform, Attackrange, null, () =>
			{

				_normalAtt.StartReady();
				_atkModule.SetAttackType(NormalAttack);


			});
			Attacker normalAttack = new Attacker(self, () =>
			{
				_normalAtt.ResetReady();
				_moveModule.StopMove();
				StopExamine();
			});

			Sequencer normalATK = new Sequencer();

			normalATK.connecteds.Add(noramlRange);
			normalATK.connecteds.Add(_normalAtt);
			normalATK.connecteds.Add(normalAttack);



			#endregion


			#region 특수2 공격
			Waiter _ex2Wait = new Waiter(7f);
			IsInRange ex2Range = new IsInRange(self, player.transform, Attackrange, null, () =>
			{

				_ex2Wait.StartReady();
				_atkModule.SetAttackType(EX2Attack);


			});
			Attacker ex2Attack = new Attacker(self, () =>
			{
				_ex2Wait.ResetReady();
				_moveModule.StopMove();
				StopExamine();
			});

			Sequencer ex2Atk = new Sequencer();

			ex2Atk.connecteds.Add(ex2Range);
			ex2Atk.connecteds.Add(_ex2Wait);
			ex2Atk.connecteds.Add(ex2Attack);


			#endregion
			
			
			
			#region 특수 공격
			Waiter _exWait = new Waiter(10f);
			IsInRange exRange = new IsInRange(self, player.transform, EX1Attackrange, null, () =>
			{

				_exWait.StartReady();
				_atkModule.SetAttackType(EXAttack);


			});
			Attacker exAttack = new Attacker(self, () =>
			{
				_exWait.ResetReady();
				_moveModule.StopMove();
				StopExamine();
			});

			Sequencer exAtk = new Sequencer();

			exAtk.connecteds.Add(exRange);
			exAtk.connecteds.Add(_exWait);
			exAtk.connecteds.Add(exAttack);


			#endregion
			
			
			#region Normal
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
			head.connecteds.Add(ex2Atk);
			head.connecteds.Add(exAtk);
			head.connecteds.Add(normalATK);
			head.connecteds.Add(ShowIdler);
			head.connecteds.Add(Moved);
			head.connecteds.Add(Faridler);



		}
		else
		{
			self.anim.SetBoolModify("Sleep", true);
		}


	}



	protected override void UpdateInvoke()
	{
		if (self.life.isDead == false && _isWake && self.life.isDead == false && self.anim.Animators.GetBool("Stun") == false)
		{
			LookAt(player.transform);

		}

		transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);

		if ((self.life.IsFirstHit == true || Vector3.Distance(player.transform.position, transform.position) < 7) && _isWake == false)
		{
			_isWake = true;
			self.anim.SetBoolModify("Sleep", false);
			StartInvoke();
		}
	}

}
