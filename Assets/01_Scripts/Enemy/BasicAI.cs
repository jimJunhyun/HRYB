using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public abstract class BasicAI : AISetter
{

	[Header("기본 이동 범위")]
	[SerializeField] public float _moveRange = 2f;

	Transform _pos;

	public float MoveRange()
	{
		return _moveRange;
	}

	protected EnemyMoveModule _moveModule;

	private void Awake()
	{
		_moveModule = self.move as EnemyMoveModule;
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


		#region Noramled

		self.life._hitEvent += _moveModule.StopMove;


		IsInRange DetectedRange = new IsInRange(self, player.transform, this.OutSectionRanged, null, () =>
		{
			Vector3 dir = (self.transform.position - player.transform.position);
			if (SectionRanged() * SectionRanged() < dir.sqrMagnitude)
				_isFind = true;
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
			_isFind = false;

		});
		IsInRange Idler = new IsInRange(self, player.transform, MoveRange, null, () =>
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

		head.connecteds.Add(ShowIdler);
		head.connecteds.Add(Moved);
		head.connecteds.Add(Faridler);


		StartExamine();

	}

}
