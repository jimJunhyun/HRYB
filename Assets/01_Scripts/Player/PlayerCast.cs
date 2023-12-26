using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum SkillSlotInfo
{
	Wood,
	Fire,
	Earth,
	Metal,
	Water,

	LClick,
	RClick,


	Max
}

public class SkillSlots
{
	SkillSlotInfo? info;
	float curCooledTime;
	public float CurCooledTime
	{
		get => curCooledTime;
		set => curCooledTime = value;
	}

	public SkillRoot skInfo;
	public bool IsUsable
	{
		get => curCooledTime >= skInfo.cooldown && skInfo.useType != SkillUseType.Passive;
	}
	public bool IsEmpty
	{
		get => skInfo == null;
	}
	public void Execute(Actor self)
	{
		if(!IsEmpty && IsUsable)
		{
			//Debug.Log($"curCool : {curCooledTime}");
			curCooledTime = 0;
			skInfo.Operate(self);
			//Debug.Log("SKILL USED");
			if(skInfo.useType == SkillUseType.ActiveConsumable)
			{
				//GameManager.instance.pinven.RemoveSkill(skInfo);
			}
		}
		else
			Debug.Log($"CurCool : {curCooledTime} / {skInfo.cooldown}");
	}

	public void Disoperate(Actor self)
	{
		if(!IsEmpty && skInfo.useType != SkillUseType.Passive)
		{
			skInfo.Disoperate(self);
		}
	}

	public void Equip(SkillRoot exec, Actor from)
	{
		skInfo = exec;
		if (!IsEmpty && info != null)
			skInfo.MySlotInfo = info.Value;
		if(exec.useType == SkillUseType.Passive)
		{
			exec.Operate(from);
		}
	}

	public void UnEquip(Actor from)
	{
		if(skInfo.useType == SkillUseType.Passive)
		{
			skInfo.Disoperate(from);
		}
		curCooledTime = 0;
		skInfo = null;
	}

	public SkillSlots(SkillRoot exec, Actor from, SkillSlotInfo? info = null)
	{
		skInfo = exec;
		this.info = info;
		if (!IsEmpty)
		{
			if(info != null)
			{
				skInfo.MySlotInfo = info.Value;
			}
			if(skInfo.useType == SkillUseType.Passive)
			{
				skInfo.Operate(from);
			}
		}
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

	public void Update()
	{
		for (int i = 0; i < ((int)WXInfo.Max); i++)
		{
			if(!slots[i].IsEmpty && slots[i].skInfo.useType != SkillUseType.Passive && !slots[i].IsUsable)
				slots[i].CurCooledTime += Time.deltaTime;
			slots[i].skInfo?.UpdateStatus();
		}
	}

	public WXSkillSlots(Actor attatched)
	{
		for (int i = 0; i < ((int)WXInfo.Max); i++)
		{
			slots[i] = new SkillSlots(null, attatched, (SkillSlotInfo)i);
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
	public WXSkillSlots slots;
	public SkillSlots lClickSlot;
	public SkillSlots rClickSlot;
	public const string LCLICKSKILL = "lClickSkill";
	public const string RCLICKSKILL = "rClickSkill";

	public const string DISOPERATE = "disop";

	public SkillSlots this[int at]
	{
		get
		{
			if(at < ((int)WXInfo.Max))
			{
				return slots[at];
			}
			else
			{
				if(at == ((int)SkillSlotInfo.LClick))
				{
					return lClickSlot;
				}
				else if(at == ((int)SkillSlotInfo.RClick))
				{
					return rClickSlot;
				}
				else
				{
					return null;
				}
			}
		}
	}


	private void Awake()
	{
		slots = new WXSkillSlots(GetActor());
		lClickSlot = new SkillSlots(null, GetActor());
		rClickSlot = new SkillSlots(null, GetActor());	
	}

	private void Start()
	{
		ConnectSkillDataTo(GameManager.instance.skillLoader["NormalBowAttack"], SkillSlotInfo.LClick);
		ConnectSkillDataTo(GameManager.instance.skillLoader["ChargeBowAttack"], SkillSlotInfo.RClick);

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
				slots[((int)SkillSlotInfo.Wood)].Execute(a);
			}
		},
		() =>
		{
			if (!slots[((int)SkillSlotInfo.Wood)].IsEmpty)
			{
				return slots[((int)SkillSlotInfo.Wood)].skInfo.castTime;
			}
			return 0;
		}));

		nameCastPair.Add(WXSkillSlots.WOODSKILL+DISOPERATE, new Preparation(
		(self) =>
		{
			if (self.TryGetComponent<Actor>(out Actor a))
			{
				slots[((int)SkillSlotInfo.Wood)].Disoperate(a);
			}
		},
		() =>
		{
			return 0;
		}));

		nameCastPair.Add(WXSkillSlots.FIRESKILL, new Preparation(
		(self) =>
		{
			if (self.TryGetComponent<Actor>(out Actor a))
			{
				if (slots[((int)SkillSlotInfo.Fire)].IsUsable)
				{
					slots[((int)SkillSlotInfo.Fire)].Execute(a);
				}
			}
		},
		() =>
		{
			if (!slots[((int)SkillSlotInfo.Fire)].IsEmpty)
			{
				return slots[((int)SkillSlotInfo.Fire)].skInfo.castTime;
			}
			return 0;
		}));

		nameCastPair.Add(WXSkillSlots.FIRESKILL + DISOPERATE, new Preparation(
		(self) =>
		{
			if (self.TryGetComponent<Actor>(out Actor a))
			{
				slots[((int)SkillSlotInfo.Fire)].Disoperate(a);
			}
		},
		() =>
		{
			return 0;
		}));

		nameCastPair.Add(WXSkillSlots.EARTHSKILL, new Preparation(
		(self) =>
		{
			if (self.TryGetComponent<Actor>(out Actor a))
			{
				if (slots[((int)SkillSlotInfo.Earth)].IsUsable)
				{
					slots[((int)SkillSlotInfo.Earth)].Execute(a);
				}
			}
		},
		() =>
		{
			if (!slots[((int)SkillSlotInfo.Earth)].IsEmpty)
			{
				return slots[((int)SkillSlotInfo.Earth)].skInfo.castTime;
			}
			return 0;
		}));

		nameCastPair.Add(WXSkillSlots.EARTHSKILL + DISOPERATE, new Preparation(
		(self) =>
		{
			if (self.TryGetComponent<Actor>(out Actor a))
			{
				slots[((int)SkillSlotInfo.Earth)].Disoperate(a);
			}
		},
		() =>
		{
			return 0;
		}));

		nameCastPair.Add(WXSkillSlots.METALSKILL, new Preparation(
		(self) =>
		{
			if (self.TryGetComponent<Actor>(out Actor a))
			{
				if (slots[((int)SkillSlotInfo.Metal)].IsUsable)
				{
					slots[((int)SkillSlotInfo.Metal)].Execute(a);
				}	
			}
		},
		() =>
		{
			if (!slots[((int)SkillSlotInfo.Metal)].IsEmpty)
			{
				return slots[((int)SkillSlotInfo.Metal)].skInfo.castTime;
			}
			return 0;
		}));

		nameCastPair.Add(WXSkillSlots.METALSKILL + DISOPERATE, new Preparation(
		(self) =>
		{
			if (self.TryGetComponent<Actor>(out Actor a))
			{
				slots[((int)SkillSlotInfo.Metal)].Disoperate(a);
			}
		},
		() =>
		{
			return 0;
		}));

		nameCastPair.Add(WXSkillSlots.WATERSKILL, new Preparation(
		(self) =>
		{
			if (self.TryGetComponent<Actor>(out Actor a))
			{
				if (slots[((int)SkillSlotInfo.Water)].IsUsable)
				{
					slots[((int)SkillSlotInfo.Water)].Execute(a);
				}
			}
		},
		() =>
		{
			if (!slots[((int)SkillSlotInfo.Water)].IsEmpty)
			{
				return slots[((int)SkillSlotInfo.Water)].skInfo.castTime;
			}
			return 0;
		}));

		nameCastPair.Add(WXSkillSlots.WATERSKILL + DISOPERATE, new Preparation(
		(self) =>
		{
			if (self.TryGetComponent<Actor>(out Actor a))
			{
				slots[((int)SkillSlotInfo.Water)].Disoperate(a);
			}
		},
		() =>
		{
			return 0;
		}));

		nameCastPair.Add(LCLICKSKILL, new Preparation(
		(self) =>
		{
			if (self.TryGetComponent<Actor>(out Actor a))
			{
				lClickSlot.Execute(a);
			}
		},
		() =>
		{
			if (!lClickSlot.IsEmpty)
			{
				return lClickSlot.skInfo.castTime;
			}
			return 0;
		}));

		nameCastPair.Add(LCLICKSKILL + DISOPERATE, new Preparation(
		(self) =>
		{
			if (self.TryGetComponent<Actor>(out Actor a))
			{
				lClickSlot.Disoperate(a);
			}
		},
		() =>
		{
			return 0;
		}));

		nameCastPair.Add(RCLICKSKILL, new Preparation(
		(self) =>
		{
			if (self.TryGetComponent<Actor>(out Actor a))
			{ 
				rClickSlot.Execute(a);
			}
		},
		() =>
		{
			if (!rClickSlot.IsEmpty)
			{
				return rClickSlot.skInfo.castTime;
			}
			return 0;
		}));

		nameCastPair.Add(RCLICKSKILL + DISOPERATE, new Preparation(
		(self) =>
		{
			if (self.TryGetComponent<Actor>(out Actor a))
			{
				rClickSlot.Disoperate(a);
			}
		},
		() =>
		{
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
		GameManager.instance.DisableCtrl();
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
		(GetActor().atk as PlayerAttack).ClearNextHit();
		GameManager.instance.uiManager.interingUI.Off();
		GameManager.instance.EnableCtrl();
	}

	private void Update()
	{
		slots.Update();
		UpdateClickSlots();

		///
		if (Input.GetKeyDown(KeyCode.Y))
		{
			ConnectSkillDataTo(GameManager.instance.skillLoader["BackKick"], SkillSlotInfo.Wood);
		}
		if (Input.GetKeyDown(KeyCode.H))
		{
			ConnectSkillDataTo(GameManager.instance.skillLoader["IceMultishot"], SkillSlotInfo.Fire);
		}
		if (Input.GetKeyDown(KeyCode.N))
		{
			DisconnectSkillDataFrom(SkillSlotInfo.Wood);
		}
		///
	}

	void UpdateClickSlots()
	{
		
		if (!lClickSlot.IsEmpty && lClickSlot.skInfo.useType != SkillUseType.Passive && !lClickSlot.IsUsable)
			lClickSlot.CurCooledTime += Time.deltaTime;
		lClickSlot.skInfo?.UpdateStatus();

		if (!rClickSlot.IsEmpty && rClickSlot.skInfo.useType != SkillUseType.Passive && !rClickSlot.IsUsable)
			rClickSlot.CurCooledTime += Time.deltaTime;
		rClickSlot.skInfo?.UpdateStatus();
	}

	public SkillRoot ConnectSkillDataTo(SkillRoot root, SkillSlotInfo to)
	{
		SkillRoot original = this[((int)to)].skInfo;
		if(original != null)
			original.Disoperate(GetActor());
		this[((int)to)].Equip(root, GetActor());
		Debug.Log($"스킬 장착 : {to}, {root.name}");
		
		return original;
	}

	public SkillRoot DisconnectSkillDataFrom(SkillSlotInfo from)
	{
		SkillRoot data = this[((int)from)].skInfo;
		if(data == null)
			return data;
		this[((int)from)].UnEquip(GetActor());
		Debug.Log($"스킬 해제 : {from}, {data.name}");
		return data;
	}

	internal void ResetSkillUse(SkillSlotInfo at)
	{
		DisoperateAt(at);
	}

	internal void ActualSkillOperate(SkillSlotInfo at)
	{
		this[((int)at)].skInfo.MyOperation(GetActor());
	}

	internal void ActualSkillDisoperate(SkillSlotInfo at)
	{
		this[((int)at)].skInfo.MyDisoperation(GetActor());
	}

	internal void ActualSkillOperate(SkillSlotInfo at, int idx)
	{
		this[((int)at)].skInfo.ActualOperateAt(GetActor(), idx);
	}

	internal void ActualSkillDisoperate(SkillSlotInfo at, int idx)
	{
		this[((int)at)].skInfo.ActualDisoperateAt(GetActor(), idx);
	}

	internal void SetSkillUse(SkillSlotInfo at)
	{
		this[((int)at)].skInfo.SetAnimations(GetActor(), at);
		UseSkillAt(at);
	}

	void UseSkillAt(SkillSlotInfo at)
	{
		if (this[((int)at)].IsUsable)
		{
			switch (at)
			{
				case SkillSlotInfo.Wood:
					Cast(WXSkillSlots.WOODSKILL);
					break;
				case SkillSlotInfo.Fire:
					Cast(WXSkillSlots.FIRESKILL);
					break;
				case SkillSlotInfo.Earth:
					Cast(WXSkillSlots.EARTHSKILL);
					break;
				case SkillSlotInfo.Metal:
					Cast(WXSkillSlots.METALSKILL);
					break;
				case SkillSlotInfo.Water:
					Cast(WXSkillSlots.WATERSKILL);
					break;
				case SkillSlotInfo.LClick:
					Cast(LCLICKSKILL);
					break;
				case SkillSlotInfo.RClick:
					Cast(RCLICKSKILL);
					break;
			}
		}
	}

	public void DisoperateAt(SkillSlotInfo at)
	{
		switch (at)
		{
			case SkillSlotInfo.Wood:
				Cast(WXSkillSlots.WOODSKILL+DISOPERATE);
				break;
			case SkillSlotInfo.Fire:
				Cast(WXSkillSlots.FIRESKILL + DISOPERATE);
				break;
			case SkillSlotInfo.Earth:
				Cast(WXSkillSlots.EARTHSKILL + DISOPERATE);
				break;
			case SkillSlotInfo.Metal:
				Cast(WXSkillSlots.METALSKILL + DISOPERATE);
				break;
			case SkillSlotInfo.Water:
				Cast(WXSkillSlots.WATERSKILL + DISOPERATE);
				break;
			case SkillSlotInfo.LClick:
				Cast(LCLICKSKILL + DISOPERATE);
				break;
			case SkillSlotInfo.RClick:
				Cast(RCLICKSKILL + DISOPERATE);
				break;
		}
	}

}
