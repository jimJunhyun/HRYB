using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public abstract class BasicAI : AISetter
{
	[Header("IsWake")][SerializeField] protected bool _isWake;

	[Header("기본 이동 범위")]
	[SerializeField] public float _moveRange = 2f;

	Transform _pos;
	Transform _target;

	public float MoveRange()
	{
		return _moveRange;
	}

	protected EnemyMoveModule _moveModule;

	private void Awake()
	{
		_moveModule = GetComponent<EnemyMoveModule>();
	}


	[Header("탐색 범위")][SerializeField] public float _sectionRange = 10f;
	[Header("초기화 범위")][SerializeField] public float _section2Range = 15f;
	bool _isFind = false;


	public float SectionRanged()
	{
		return _sectionRange;
	}

	public float OutSectionRanged()
	{
		return _section2Range;
	}

	public override void StartInvoke()
	{
		_pos = new GameObject($"{gameObject.name} originPos").transform;
		_pos.transform.position = transform.position;


		#region Noramled

		self.life._hitEvent += _moveModule.StopMove;


		IsInRange DetectedRange = new IsInRange(self, player.transform, this.OutSectionRanged, null, () =>
		{
			Vector3 dir = (self.transform.position - player.transform.position);
			if (SectionRanged() * SectionRanged() < dir.sqrMagnitude)
			{
				_isFind = true;
				_target = player.transform;
			}
			if (_isFind)
				_moveModule.SetTarget(player.transform);
		});
		Mover move = new Mover(self);

		Sequencer Moved = new Sequencer();
		Moved.connecteds.Add(DetectedRange);
		Moved.connecteds.Add(move);

		IsOutRange LongaRange = new IsOutRange(self, player.transform, OutSectionRanged, null, () =>
		{
			_moveModule.SetTarget(_pos);
			_target = _pos;
			_isFind = false;

		});
		Mover originreturn = new Mover(self);

		IsInRange Idler = new IsInRange(self, player.transform, MoveRange, null, () =>
		{
			_moveModule.StopMove();
		});

		Idler idles = new Idler(self);

		Sequencer Faridler = new Sequencer();
		Faridler.connecteds.Add(LongaRange);
		Faridler.connecteds.Add(originreturn);

		Sequencer ShowIdler = new Sequencer();

		ShowIdler.connecteds.Add(Idler);
		ShowIdler.connecteds.Add(idles);
		#endregion

		head.connecteds.Add(ShowIdler);
		head.connecteds.Add(Moved);
		head.connecteds.Add(Faridler);


		StartExamine();

	}

	protected override void UpdateInvoke()
	{
		if ((self.life.IsFirstHit == true || Vector3.Distance(_target.transform.position, transform.position) < 7) && _isWake == false)
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
