using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangGwiAI : AISetter
{

	[Header("공격 시작 범위")]
	[SerializeField] public float _attackRange = 2f;
	[SerializeField] public float _dashAttackRange = 8f;

	[Header("탐색 범위")][SerializeField] public float _sectionRange = 10f;
	[Header("초기화 범위")][SerializeField] public float _section2Range = 15f;



	private const string NormalAttack = "NormallAtt";
	private const string DashAttack = "DashAtt";

	public override void DieEvent(float delay = 0, float time = 3)
	{
		//self.anim.ResetStatus();
		StopExamine();
		ChangGwiMoveModule _moveModule = self.move as ChangGwiMoveModule;
		GetComponent<BoxCollider>().enabled = false;
		_moveModule.StopMove();
		base.DieEvent();
	}

	public override void LookAt(Transform t)
	{
		Vector3 lookPos = t.position - transform.position;
		lookPos.y = transform.position.y;
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookPos), Time.deltaTime * 4);
	}


	public override void StartInvoke()
	{
		head.connecteds.Clear();

		ChangGwiAttackModule _atkModule = self.atk as ChangGwiAttackModule;
		ChangGwiMoveModule _moveModule = self.move as ChangGwiMoveModule;


		IsNotStarted = true;
		self.anim.SetIdleState(true);
		self.life._dieEvent += () => { DieEvent(); };
		self.life._hitEvent += _moveModule.StopMove;

		StunNode _ishaveStun = new StunNode(self, () =>
		{
			//Debug.LogError(gameObject.name + " 일어남");//
		});
		Sequencer stunSeq = new Sequencer();

		stunSeq.connecteds.Add(_ishaveStun);


		#region 대쉬
		Waiter _dashWait = new Waiter(1.5f);

		IsInRange _dashRange = new IsInRange(self, player.transform, DashAttackRange, null, () =>
		{
			_dashWait.StartReady();
			_atkModule.SetAttackType(DashAttack);
			Debug.LogError("이거 왜안됨???");
			_moveModule.StopMove();
		});

		Attacker _dashAttack = new Attacker(self, () =>
		{
			_dashWait.ResetReady();
			StopExamine();
		});

		Sequencer dashSeq = new Sequencer();
		dashSeq.connecteds.Add(_dashRange);
		dashSeq.connecteds.Add(_dashWait);
		dashSeq.connecteds.Add(_dashAttack);


		#endregion

		#region 평타

		Waiter _normalWait = new Waiter(2f);

		IsInRange noramlRange = new IsInRange(self, player.transform, Attackrange, null, () =>
		{
			_normalWait.StartReady();
			_atkModule.SetAttackType(NormalAttack);
			_moveModule.StopMove();
		});

		Attacker normalAttack = new Attacker(self, () =>
		{
			_normalWait.ResetReady();
			StopExamine();
		});

		Sequencer normalATK = new Sequencer();

		normalATK.connecteds.Add(noramlRange);
		normalATK.connecteds.Add(_normalWait);
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

		head.connecteds.Add(stunSeq);
		//head.connecteds.Add(dashSeq);
		head.connecteds.Add(normalATK);
		head.connecteds.Add(ShowIdler);
		head.connecteds.Add(Moved);
		head.connecteds.Add(Faridler);

		StartExamine();
	}

	protected override void UpdateInvoke()
	{
		if (self.AI.StopState)
			return;

		if (self.life.isDead == false && self.anim.Animators.GetBool("Stun") == false)
		{
			LookAt(player.transform);

		}

		transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
	}


	public float Attackrange()
	{
		return _attackRange;
	}

	public float DashAttackRange()
	{
		return _dashAttackRange;
	}

	public float SectionRanged()
	{
		return _sectionRange;
	}

	public float OutSectionRanged()
	{
		return _section2Range;
	}


}
