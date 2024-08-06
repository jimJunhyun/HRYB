using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[RequireComponent(typeof(AttackModule))]
[RequireComponent(typeof(MoveModule))]
[RequireComponent(typeof(LifeModule))]
[RequireComponent(typeof(SightModule))]
[RequireComponent(typeof(CastModule))]
[RequireComponent(typeof(AnimModule))]
[RequireComponent(typeof(WalletModule))]
public class Actor : MonoBehaviour
{
	public AttackModule atk;
	public MoveModule move;
	public LifeModule life;
	public SightModule sight;
	public CastModule cast;
	public AnimModule anim;
	public WalletModule wallet;

	public TalkModule talk;
	public MinimapTarget show;

	public Action<Actor> updateActs;
	public AISetter _ai;

	public string actorName;



	List<TemporaryStatMods> ongoingTempStatMods = new List<TemporaryStatMods>();

	public AISetter AI
	{
		get
		{
			if(_ai == null && TryGetComponent(out AISetter value))
			{
				_ai = value;
			}
			else if (_ai == null)
			{
				return null;	
			}

			return _ai;
		}
	}

	private void Awake()
	{
		atk = GetComponent<AttackModule>();
		move = GetComponent<MoveModule>();
		life = GetComponent<LifeModule>();
		sight = GetComponent<SightModule>();
		cast = GetComponent<CastModule>();
		anim = GetComponent<AnimModule>();

		talk = GetComponent<TalkModule>();
		show = GetComponent<MinimapTarget>();
		wallet = GetComponent<WalletModule>();

		if(actorName == null || actorName == "")
			actorName = transform.name;
	}
	void Start()
	{
		Respawn();
	}

	private void Update()
	{
		updateActs?.Invoke(this);
	}

	public virtual void Respawn()
	{
		_ai?.ResetStatus();
		atk.ResetStatus();
		move.ResetStatus();
		life.ResetStatus();
		sight.ResetStatus();
		cast.ResetStatus();
		anim.ResetStatus();
		talk?.ResetStatus();
		show?.ResetStatus();
		wallet.ResetStatus();

		for (int i = 0; i < ongoingTempStatMods.Count; i++)
		{
			HandleStatus(ongoingTempStatMods[i], true);
			StopCoroutine(ongoingTempStatMods[i].c);
		}

		ongoingTempStatMods.Clear();
	}

	public void AddStat(float amt, StatUpgradeType type)
	{
		switch (type)
		{
			case StatUpgradeType.White:
				life.yy.white.AddMod(amt);
				break;
			case StatUpgradeType.Black:
				life.yy.black.AddMod(amt);
				break;
			case StatUpgradeType.WhiteAtk:
				atk.Damage.white.AddMod(amt);
				break;
			case StatUpgradeType.BlackAtk:
				atk.Damage.black.AddMod(amt);
				break;
			case StatUpgradeType.MoveSpeed:
				move.runSpeed.AddMod( amt);
				move.walkSpeed.AddMod( amt);
				move.crouchSpeed.AddMod( amt);
				break;
			case StatUpgradeType.CooldownRdc:
				//cast.cooldownModuleStat.HandleSpeed(-amt, ModuleController.SpeedMode.Slow);
				break;
			case StatUpgradeType.Callback:
				break;
			default:
				break;
		}
	}

	public void MultStat(float amt, StatUpgradeType type)
	{
		switch (type)
		{
			case StatUpgradeType.White:
				life.yy.white.MultMod(amt);
				break;
			case StatUpgradeType.Black:
				life.yy.black.MultMod(amt);
				break;
			case StatUpgradeType.WhiteAtk:
				atk.Damage.white.MultMod(amt);
				break;
			case StatUpgradeType.BlackAtk:
				atk.Damage.black.MultMod(amt);
				break;
			case StatUpgradeType.MoveSpeed:
				move.runSpeed.MultMod(amt);
				move.walkSpeed.MultMod(amt);
				move.crouchSpeed.MultMod(amt);
				move.RefreshSpeed();
				break;
			case StatUpgradeType.CooldownRdc:
				(cast as PlayerCast).cooldownModuleStat.HandleSpeed(-amt, ModuleController.SpeedMode.Slow);
				break;
			case StatUpgradeType.Callback:
				break;
			default:
				break;
		}
	}

	public void HandleStatus(StatUpgradeType type, float amtAdd, float amtMult, float timePeriod)
	{
		if(timePeriod == Mathf.Infinity || timePeriod < 0)
		{
			AddStat(amtAdd, type);
			MultStat(amtAdd, type);
		}
		else
		{
			ongoingTempStatMods.Add(new TemporaryStatMods(StartCoroutine(DelHandleStat(type, amtAdd, amtMult, timePeriod)), type, amtAdd, amtMult));
		}
	}

	void HandleStatus(TemporaryStatMods mods, bool invert)
	{
		if (invert)
		{
			AddStat(-mods.amtAdd, mods.type);
			MultStat(-mods.amtMult, mods.type);
		}
		else
		{
			AddStat(mods.amtAdd, mods.type);
			MultStat(mods.amtMult, mods.type);
		}
		
	}

	IEnumerator DelHandleStat(StatUpgradeType type, float amtAdd, float amtMult, float time)
	{
		HandleStatus(type, amtAdd, amtMult, -1);
		float t = 0;
		while(t <= time)
		{
			yield return null;
			t += Time.deltaTime;
		}
		yield return null;
		HandleStatus(type, -amtAdd, -amtMult, -1);
	}
}

public struct TemporaryStatMods
{
	public Coroutine c;
	public StatUpgradeType type;
	public float amtAdd;
	public float amtMult;
	public TemporaryStatMods(Coroutine ongoing, StatUpgradeType t, float amtAdd, float amtMult)
	{
		c = ongoing;
		type = t;
		this.amtAdd = amtAdd;
		this.amtMult = amtMult;
	}
}

public class UpgradableStatus
{
	float maxValue;
	float modValueAdd;
	float modValueMult;

	float value;

	int roundThreshold;

	public float Value
	{
		get =>value;
		set
		{
			this.value = value;
			this.value = Mathf.Clamp(this.value, 0, MaxValue);
		}
	}
	public float MaxValue
	{
		get => Mathf.Round(((maxValue + modValueAdd) * (1 + modValueMult)) * Mathf.Pow(10, roundThreshold)) / Mathf.Pow(10, roundThreshold);
	}

	public void AddMod(float amt)
	{
		float prevVal = MaxValue;
		modValueAdd += amt;


		if(MaxValue - prevVal > 0)
		{
			value += MaxValue - prevVal;
		}
		value = Mathf.Clamp(value, 0, MaxValue);
	}

	public void MultMod(float amt)
	{
		float prevVal = MaxValue;
		modValueMult += amt;

		if (MaxValue - prevVal > 0)
		{
			value += MaxValue - prevVal;
		}
		value = Mathf.Clamp(value, 0, MaxValue);
	}

	public void ResetCompletely()
	{
		modValueAdd = 0;
		modValueMult = 0;
		value = MaxValue;

	}

	public UpgradableStatus(int roundFrom, float initValue)
	{
		maxValue = initValue;
		modValueAdd= 0;
		modValueMult = 0;

		roundThreshold = roundFrom;
		value = MaxValue;

	}

	public UpgradableStatus(UpgradableStatus origin)
	{
		value = origin.value;
		modValueAdd = origin.modValueAdd;
		modValueMult = origin.modValueMult;
		roundThreshold = origin.roundThreshold;
		maxValue = origin.maxValue;
	}
}
