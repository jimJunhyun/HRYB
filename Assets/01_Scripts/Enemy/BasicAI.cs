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

	public float HomeRangedFunc()
	{
		return 3f;
	}

	protected EnemyMoveModule _moveModule;

	private void Awake()
	{

		_pos = new GameObject($"{gameObject.name} originPos").transform;
		_pos.transform.position = transform.position;
		_target = _pos;

		_moveModule = GetComponent<EnemyMoveModule>();
		transform.localEulerAngles = new Vector3(0, Random.Range(0.0f,360.0f),0);
	//	Debug.LogError($"Random Eultr {transform.localEulerAngles}");
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

	public void STSetting()
	{


		#region Noramled

		self.life._hitEvent += _moveModule.StopMove;


		IsInRange DetectedRange = new IsInRange(self, player.transform, this.OutSectionRanged, null, () =>
		{
			Vector3 dir = (self.transform.position - player.transform.position);

			//Debug.LogError($" A : {dir.sqrMagnitude} > B {SectionRanged() * SectionRanged()}");
			if (SectionRanged() * SectionRanged() > dir.sqrMagnitude)
			{
				_isFind = true;
				_target = player.transform;
			}

			if (_isFind)
				_moveModule.SetTarget(player.transform);
			else
				_moveModule.StopMove();
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


		IsInRange HomeRange = new IsInRange(self, _pos.transform, HomeRangedFunc, null, () =>
		{
			_moveModule.StopMove();
			self.anim.SetIdleState(true);
			self.life.yy.white.Value = self.life.initWhite;
		});


		IsInRange Idler = new IsInRange(self, player.transform, MoveRange, null, () =>
		{
			_moveModule.StopMove();
			self.anim.SetIdleState(true);
		});

		Idler idles = new Idler(self);


		Sequencer ReturnHomeSeq = new Sequencer();
		ReturnHomeSeq.connecteds.Add(LongaRange);
		ReturnHomeSeq.connecteds.Add(originreturn);

		Sequencer IsHomeSeq = new Sequencer();
		IsHomeSeq.connecteds.Add(HomeRange);
		IsHomeSeq.connecteds.Add(idles);


		//Sequencer Isgoing = new Sequencer();
		//Isgoing.connecteds.Add(IsHomeSeq);
		//Isgoing.connecteds.Add(ReturnHomeSeq);

		

		Sequencer ShowIdler = new Sequencer();

		ShowIdler.connecteds.Add(Idler);
		ShowIdler.connecteds.Add(idles);
		#endregion

		head.connecteds.Add(ShowIdler);
		head.connecteds.Add(Moved);
		head.connecteds.Add(IsHomeSeq);
		head.connecteds.Add(ReturnHomeSeq);

		IsNotStarted = true;


		StartExamine();

	}

	protected override void UpdateInvoke()
	{
		if ((self.life.IsFirstHit == true || Vector3.Distance(player.transform.position, transform.position) < 7) && _isWake == false)
		{
			_isWake = true;
			self.anim.SetBoolModify("Sleep", false);
			StartInvoke();
			self.anim.SetIdleState(true);
		}


		if (self.AI.StopState)
			return;

		if (self.life.isDead == false && _isWake && self.life.isDead == false && self.anim.Animators.GetBool("Stun") == false)
		{
			LookAt(_target.transform);

		}

		transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);

	}

}
