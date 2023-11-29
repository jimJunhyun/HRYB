using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSlots
{
	WXInfo info;
	float curCooledTime;
	public float CurCooledTime
	{
		get => curCooledTime;
		set => curCooledTime = value;
	}

	public SkillRoot skInfo;
	public bool IsUsable
	{
		get => curCooledTime >= skInfo.cooldown;
	}
	public bool IsEmpty
	{
		get => skInfo == null;
	}
	public void Execute(Actor self)
	{
		if(!IsEmpty && skInfo.cooldown <= curCooledTime)
		{
			curCooledTime = 0;
			skInfo.Operate(self);
		}
	}

	public SkillSlots(WXInfo info, SkillRoot exec)
	{
		this.info = info;
		skInfo = exec;
		if(!IsEmpty)
			skInfo.MySlotInfo = this.info;
		curCooledTime = 0;
	}
}

public class WXSkillSlots
{
	public const string WOODSKILL = "woodSkill";
	public const string FIRESKILL = "fireSkill";
	public const string EARTHSKILL = "earthSkill";
	public const string METALSKILL = "metalSkill";
	public const string WATERSKILL = "waterSkill";

	SkillSlots[] slots = new SkillSlots[5];

	public void Update(float dt)
	{
		for (int i = 0; i < ((int)WXInfo.Max); i++)
		{
			if(!slots[i].IsEmpty && !slots[i].IsUsable)
				slots[i].CurCooledTime += dt;
		}
	}

	public WXSkillSlots()
	{
		for (int i = 0; i < ((int)WXInfo.Max); i++)
		{
			slots[i] = new SkillSlots((WXInfo)i, null);
		}
	}
	public SkillSlots this[int i]
	{
		get
		{
			if(i < ((int)WXInfo.Max))
			{
				return slots[i];
			}
			return null;
		}
	}
}

public class PlayerCast : CastModule
{
	public WXSkillSlots slots = new WXSkillSlots();

	private void Start()
	{
		nameCastPair.Add("interact" , new Preparation(
		(self)=>
		{
			if((GetActor().sight as PlayerInter).curFocused != null)
			{
				//GameManager.instance.uiManager.debugText.text += $"  FUNC EXECUTING {(GetActor().sight as PlayerInter).curFocused}...  ";
				(GetActor().sight as PlayerInter).curFocused.InteractWith();
				(GetActor().sight as PlayerInter).Check();
				
			}
		},
		() =>
		{
			if((GetActor().sight as PlayerInter).curFocused != null)
			{
				//Debug.Log($"delSec : {GameManager.instance.pinter.curFocused.InterTime}");
				float t = (GetActor().sight as PlayerInter).curFocused.InterTime;
				(GetActor().anim as PlayerAnim).SetInterSpeed((1 / t) * castMod);
				return t;
			}
			return 0;
		}));

		nameCastPair.Add("altInteract", new Preparation(
		(self) =>
		{
			if ((GetActor().sight as PlayerInter).curFocused != null && (GetActor().sight as PlayerInter).curFocused.AltInterable)
			{
				(GetActor().sight as PlayerInter).curFocused.AltInterWith();
				(GetActor().sight as PlayerInter).Check();
			}
		},
		() =>
		{
			if ((GetActor().sight as PlayerInter).curFocused != null)
			{
				float t = (GetActor().sight as PlayerInter).curFocused.InterTime;
				(GetActor().anim as PlayerAnim).SetInterSpeed((1 / t) * castMod);
				return t;
			}
			return 0;
		}));

		nameCastPair.Add(WXSkillSlots.WOODSKILL, new Preparation(
		(self)=>
		{
			if(self.TryGetComponent<Actor>(out Actor a))
			{
				slots[((int)WXInfo.Wood)].Execute(a);
			}
		},
		() =>
		{
			if (!slots[((int)WXInfo.Wood)].IsEmpty)
			{
				return slots[((int)WXInfo.Wood)].skInfo.cooldown;
			}
			return 0;
		}));

		nameCastPair.Add(WXSkillSlots.FIRESKILL, new Preparation(
		(self) =>
		{
			if (self.TryGetComponent<Actor>(out Actor a))
			{
				slots[((int)WXInfo.Fire)].Execute(a);
			}
		},
		() =>
		{
			if (!slots[((int)WXInfo.Fire)].IsEmpty)
			{
				return slots[((int)WXInfo.Fire)].skInfo.cooldown;
			}
			return 0;
		}));

		nameCastPair.Add(WXSkillSlots.EARTHSKILL, new Preparation(
		(self) =>
		{
			if (self.TryGetComponent<Actor>(out Actor a))
			{
				slots[((int)WXInfo.Earth)].Execute(a);
			}
		},
		() =>
		{
			if (!slots[((int)WXInfo.Earth)].IsEmpty)
			{
				return slots[((int)WXInfo.Earth)].skInfo.cooldown;
			}
			return 0;
		}));

		nameCastPair.Add(WXSkillSlots.METALSKILL, new Preparation(
		(self) =>
		{
			if (self.TryGetComponent<Actor>(out Actor a))
			{
				slots[((int)WXInfo.Metal)].Execute(a);
			}
		},
		() =>
		{
			if (!slots[((int)WXInfo.Metal)].IsEmpty)
			{
				return slots[((int)WXInfo.Metal)].skInfo.cooldown;
			}
			return 0;
		}));

		nameCastPair.Add(WXSkillSlots.WATERSKILL, new Preparation(
		(self) =>
		{
			if (self.TryGetComponent<Actor>(out Actor a))
			{
				slots[((int)WXInfo.Water)].Execute(a);
			}
		},
		() =>
		{
			if (!slots[((int)WXInfo.Water)].IsEmpty)
			{
				return slots[((int)WXInfo.Water)].skInfo.cooldown;
			}
			return 0;
		}));

	}

	public override void CastCancel()
	{
		base.CastCancel();
		GameManager.instance.uiManager.interingUI.Off();
		GameManager.instance.pinp.ActivateInput();
	}

	protected override IEnumerator DelCast(Preparation p)
	{
		GameManager.instance.pinp.DeactivateInput();
		GameManager.instance.uiManager.interingUI.On();
		float t = 0;
		float waitSec = p.Timefunc();
		while (waitSec * castMod > t)
		{
			t += Time.deltaTime;
			GameManager.instance.uiManager.interingUI.SetGaugeValue(t / (waitSec * castMod));
			yield return null;
		}
		p.onPrepComp?.Invoke(transform);
		ongoing = null;
		curName = null;
		GameManager.instance.uiManager.interingUI.Off();
		GameManager.instance.pinp.ActivateInput();
	}

	private void Update()
	{
		slots.Update(Time.deltaTime);
		if (Input.GetKeyDown(KeyCode.Y))
		{
			ConnectSkillDataTo(GameManager.instance.skillLoader.skillDb.info["DemoSkill"], WXInfo.Wood);
		}
		if (Input.GetKeyDown(KeyCode.H))
		{
			DisconnectSkillDataFrom(WXInfo.Wood);
		}
	}

	public void ConnectSkillDataTo(SkillRoot root, WXInfo to)
	{
		slots[((int)to)].skInfo = root;
		Debug.Log($"스킬 장착 : {to}, {root.name}");
	}

	public SkillRoot DisconnectSkillDataFrom(WXInfo from)
	{
		SkillRoot data = null;
		data = slots[((int)from)].skInfo;
		data.Disoperate(GetActor());
		slots[((int)from)].skInfo = null;
		slots[((int)from)].CurCooledTime = 0;
		Debug.Log($"스킬 해제 : {from}, {data.name}");
		return data;
	}
}
