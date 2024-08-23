using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldugiAI : BasicAI
{
	[Header("공격 시작 범위")]
	[SerializeField] public float _attackRange = 2f;

	private const string NormalAttack = "NormalAttack";

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

		self.anim.SetIdleState(true);
		self.life._dieEvent += () => { DieEvent(); };
		self.life._hitEvent += _moveModule.StopMove;

		Waiter _normalWait = new Waiter(2f);

		IsInRange noramlRange = new IsInRange(self, player.transform, Attackrange, null, () =>
		{
			_normalWait.StartReady();
			//_atkModule.SetAttackType(NormalAttack);
			//_moveModule.StopMove();
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
}
