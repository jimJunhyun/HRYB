using UnityEngine;
using UnityEngine.AI;

public class EnemyMoveModule : MoveModule
{
	Transform _target;


	private bool _isMove = false;
	UnityEngine.AI.NavMeshAgent _agent;

	NavMeshAgent agent
	{
		get
		{
			if (_agent == null)
			{
				_agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
			}

			return _agent;
		}
	}
	private CharacterController _char;

	public UnityEngine.AI.NavMeshAgent Agent => agent;
	public CharacterController Character => _char;

	public override MoveStates moveStat
	{
		get => curStat;
		protected set
		{
			switch (value)
			{
				case MoveStates.Walk:
					curStat = MoveStates.Walk;
					speed = walkSpeed.MaxValue;
					break;
				case MoveStates.Run:
					curStat = MoveStates.Run;
					speed = runSpeed.MaxValue;
					break;
				case MoveStates.Sit:
					curStat = MoveStates.Sit;
					speed = crouchSpeed.MaxValue;
					break;
				default:
					break;
			}

			agent.speed = speed;
		}
	}

	public override void Awake()
	{
		base.Awake();
		_char = GetComponent<CharacterController>();
		agent.speed = speed;
	}

	public void SetTarget(Transform target, MoveStates moves = MoveStates.Run)
	{
		this._target = target;
		if(agent.enabled)
		{
			moveStat = moves;
			agent.isStopped = false;
			agent.updatePosition = true;
			agent.updateRotation = false;
		}
	}

	public override void Move()
	{
		_isMove = true;

		if (_isMove == true && _target != null && agent.enabled == true)
		{

			self.anim.SetMoveState(true);
			UnityEngine.AI.NavMesh.SamplePosition(_target.transform.position, out UnityEngine.AI.NavMeshHit hit, 1f, UnityEngine.AI.NavMesh.AllAreas);
			agent.SetDestination(hit.position);
		}
		else
		{
			self.anim.SetMoveState(false);
			StopMove();
		}
	}

	public override void FixedUpdate()
	{

		if(GetActor().life.isDead == false)
		{
			ForceCalc();
			GravityCalc();
			Character.Move((forceDir) * Time.deltaTime);
			if (_isCanMove != true)
			{
				if ((forceDir.y > 0) || isGrounded == false)
				{
					Character.enabled = true;
					agent.enabled = false;
				}
				else
				{
					Character.enabled = false;
					agent.enabled = true;
				}
			}
		}
		else
		{
			if(agent.enabled)
			{
				StopMove();
				agent.enabled = false;
			}
			Character.enabled = true;
			ForceCalc();
			GravityCalc();
			Character.Move((forceDir) * Time.deltaTime);

		}



	}


	public void StopMove()
	{
		if(agent.enabled == true)
		{

			agent.isStopped = true;
			agent.updatePosition = false;
			agent.updateRotation = false;
			agent.velocity = new Vector3(0, 0, 0);

		}

		_isMove = false;
		SetTarget(transform);
		self.anim.SetMoveState(false);
	}

	public override void ResetStatus()
	{
		base.ResetStatus();
		StopMove();
	}
}
