using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public enum SkillSlotInfo
{
	One,
	Two,
	Three,
	Q,
	E,

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

	public void CooldownReductionSolid(float amt)
	{
		curCooledTime += amt;
	}

	public void CooldownReductionPercent(float amt)
	{
		curCooledTime += skInfo.cooldown * amt;
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
	

	SkillSlots[] slots = new SkillSlots[(int)SkillSlotInfo.Max];

	public void Update()
	{
		for (int i = 0; i < ((int)SkillSlotInfo.Max); i++)
		{
			if(!slots[i].IsEmpty && slots[i].skInfo.useType != SkillUseType.Passive && !slots[i].IsUsable)
				slots[i].CurCooledTime += Time.deltaTime;
			slots[i].skInfo?.UpdateStatus();
		}
	}

	public WXSkillSlots(Actor attatched)
	{
		for (int i = 0; i < ((int)SkillSlotInfo.Max); i++)
		{
			slots[i] = new SkillSlots(null, attatched, (SkillSlotInfo)i);
		}
	}
	public SkillSlots this[int i]
	{
		get
		{
			if(i < ((int)SkillSlotInfo.Max))
			{
				return slots[i];
			}
			return null;
		}
	}
}

public class PlayerCast : CastModule
{
	public WXSkillSlots nowSkillSlot;
	public WXSkillSlots humenSkillSlot;
	public WXSkillSlots yohoSkillSlot;
	
	public const string WOODSKILL = "woodSkill";
	public const string FIRESKILL = "fireSkill";
	public const string EARTHSKILL = "earthSkill";
	public const string METALSKILL = "metalSkill";
	public const string WATERSKILL = "waterSkill";
	public const string LCLICKSKILL = "lClickSkill";
	public const string RCLICKSKILL = "rClickSkill";

	public const string DISOPERATE = "disop";
	public const string LOOP = "Loop";

	private SkillRoot _nowSkillUse= null;
	public SkillRoot NowSkillUse => _nowSkillUse;


	private void Awake()
	{
		humenSkillSlot = new WXSkillSlots(GetActor());
		yohoSkillSlot = new WXSkillSlots(GetActor());
		
		nowSkillSlot = humenSkillSlot;
	}

	private void Start()
	{
		ConnectSkillDataTo(GameManager.instance.skillLoader.GetHumenSkill("NormalBowAttack"), SkillSlotInfo.LClick, PlayerForm.Magic);
		ConnectSkillDataTo(GameManager.instance.skillLoader.GetHumenSkill("ChargeBowAttack"), SkillSlotInfo.RClick, PlayerForm.Magic);
		
		//ConnectSkillDataTo(GameManager.instance.skillLoader.GetYohoSkill("YohoNormalAttack"), SkillSlotInfo.LClick, PlayerForm.Magic);
		//ConnectSkillDataTo(GameManager.instance.skillLoader.GetYohoSkill("NextEnter"), SkillSlotInfo.RClick, PlayerForm.Magic);
		//ConnectSkillDataTo(GameManager.instance.skillLoader.GetYohoSkill("SkyBritgh"), SkillSlotInfo.One, PlayerForm.Magic);
		//ConnectSkillDataTo(GameManager.instance.skillLoader.GetYohoSkill("YusungSmith"), SkillSlotInfo.Three, PlayerForm.Magic);
		//ConnectSkillDataTo(GameManager.instance.skillLoader.GetYohoSkill("YohoSharpnessAtt"), SkillSlotInfo.Two, PlayerForm.Magic);
		// YohoNormalAttack
		
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
				(GetActor().anim as PlayerAnim).SetInterSpeed((1 / t) * castModuleStat.Speed);
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
				(GetActor().anim as PlayerAnim).SetInterSpeed((1 / t) * castModuleStat.Speed);
				return t;
			}
			return 0;
		}));

		nameCastPair.Add(WOODSKILL, new Preparation(
		(self)=>
		{
			if(self.TryGetComponent<Actor>(out Actor a))
			{
				nowSkillSlot[((int)SkillSlotInfo.One)].Execute(a);
				_nowSkillUse = nowSkillSlot[((int)SkillSlotInfo.One)].skInfo;
			}
		},
		() =>
		{
			if (!nowSkillSlot[((int)SkillSlotInfo.One)].IsEmpty)
			{
				return nowSkillSlot[((int)SkillSlotInfo.One)].skInfo.castTime;
			}
			return 0;
		}));

		nameCastPair.Add(WOODSKILL+DISOPERATE, new Preparation(
		(self) =>
		{
			if (self.TryGetComponent<Actor>(out Actor a))
			{
				nowSkillSlot[((int)SkillSlotInfo.One)].Disoperate(a);
			}
		},
		() =>
		{
			return 0;
		}));

		nameCastPair.Add(FIRESKILL, new Preparation(
		(self) =>
		{
			if (self.TryGetComponent<Actor>(out Actor a))
			{
				if (nowSkillSlot[((int)SkillSlotInfo.Two)].IsUsable)
				{
					nowSkillSlot[((int)SkillSlotInfo.Two)].Execute(a);
					_nowSkillUse = nowSkillSlot[((int)SkillSlotInfo.Two)].skInfo;
				}
			}
		},
		() =>
		{
			if (!nowSkillSlot[((int)SkillSlotInfo.Two)].IsEmpty)
			{
				return nowSkillSlot[((int)SkillSlotInfo.Two)].skInfo.castTime;
			}
			return 0;
		}));

		nameCastPair.Add(FIRESKILL + DISOPERATE, new Preparation(
		(self) =>
		{
			if (self.TryGetComponent<Actor>(out Actor a))
			{
				nowSkillSlot[((int)SkillSlotInfo.Two)].Disoperate(a);
			}
		},
		() =>
		{
			return 0;
		}));

		nameCastPair.Add(EARTHSKILL, new Preparation(
		(self) =>
		{
			if (self.TryGetComponent<Actor>(out Actor a))
			{
				if (nowSkillSlot[((int)SkillSlotInfo.Three)].IsUsable)
				{
					nowSkillSlot[((int)SkillSlotInfo.Three)].Execute(a);
					_nowSkillUse = nowSkillSlot[((int)SkillSlotInfo.Three)].skInfo;
				}
			}
		},
		() =>
		{
			if (!nowSkillSlot[((int)SkillSlotInfo.Three)].IsEmpty)
			{
				return nowSkillSlot[((int)SkillSlotInfo.Three)].skInfo.castTime;
			}
			return 0;
		}));

		nameCastPair.Add(EARTHSKILL + DISOPERATE, new Preparation(
		(self) =>
		{
			if (self.TryGetComponent<Actor>(out Actor a))
			{
				nowSkillSlot[((int)SkillSlotInfo.Three)].Disoperate(a);
			}
		},
		() =>
		{
			return 0;
		}));

		nameCastPair.Add(METALSKILL, new Preparation(
		(self) =>
		{
			if (self.TryGetComponent<Actor>(out Actor a))
			{
				if (nowSkillSlot[((int)SkillSlotInfo.Q)].IsUsable)
				{
					nowSkillSlot[((int)SkillSlotInfo.Q)].Execute(a);
					_nowSkillUse = nowSkillSlot[((int)SkillSlotInfo.Q)].skInfo;
				}	
			}
		},
		() =>
		{
			if (!nowSkillSlot[((int)SkillSlotInfo.Q)].IsEmpty)
			{
				return nowSkillSlot[((int)SkillSlotInfo.Q)].skInfo.castTime;
			}
			return 0;
		}));

		nameCastPair.Add(METALSKILL + DISOPERATE, new Preparation(
		(self) =>
		{
			if (self.TryGetComponent<Actor>(out Actor a))
			{
				nowSkillSlot[((int)SkillSlotInfo.Q)].Disoperate(a);
			}
		},
		() =>
		{
			return 0;
		}));

		nameCastPair.Add(WATERSKILL, new Preparation(
		(self) =>
		{
			if (self.TryGetComponent<Actor>(out Actor a))
			{
				if (nowSkillSlot[((int)SkillSlotInfo.E)].IsUsable)
				{
					nowSkillSlot[((int)SkillSlotInfo.E)].Execute(a);
					_nowSkillUse = nowSkillSlot[((int)SkillSlotInfo.E)].skInfo;
				}
			}
		},
		() =>
		{
			if (!nowSkillSlot[((int)SkillSlotInfo.E)].IsEmpty)
			{
				return nowSkillSlot[((int)SkillSlotInfo.E)].skInfo.castTime;
			}
			return 0;
		}));

		nameCastPair.Add(WATERSKILL + DISOPERATE, new Preparation(
		(self) =>
		{
			if (self.TryGetComponent<Actor>(out Actor a))
			{
				nowSkillSlot[((int)SkillSlotInfo.E)].Disoperate(a);
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
				//Debug.Log("CASTED");
				nowSkillSlot[(int)SkillSlotInfo.LClick].Execute(a);
				_nowSkillUse = nowSkillSlot[((int)SkillSlotInfo.LClick)].skInfo;
			}
		},
		() =>
		{
			if (!nowSkillSlot[(int)SkillSlotInfo.LClick].IsEmpty)
			{
				return nowSkillSlot[(int)SkillSlotInfo.LClick].skInfo.castTime;
			}
			return 0;
		}));

		nameCastPair.Add(LCLICKSKILL + DISOPERATE, new Preparation(
		(self) =>
		{
			if (self.TryGetComponent<Actor>(out Actor a))
			{
				nowSkillSlot[(int)SkillSlotInfo.LClick].Disoperate(a);
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
				nowSkillSlot[(int)SkillSlotInfo.RClick].Execute(a);
				_nowSkillUse = nowSkillSlot[((int)SkillSlotInfo.RClick)].skInfo;
			}
		},
		() =>
		{
			if (!nowSkillSlot[(int)SkillSlotInfo.RClick].IsEmpty)
			{
				return nowSkillSlot[(int)SkillSlotInfo.RClick].skInfo.castTime;
			}
			return 0;
		}));

		nameCastPair.Add(RCLICKSKILL + DISOPERATE, new Preparation(
		(self) =>
		{
			if (self.TryGetComponent<Actor>(out Actor a))
			{
				nowSkillSlot[(int)SkillSlotInfo.RClick].Disoperate(a);
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
		GameManager.instance.DisableCtrl(ControlModuleMode.Animated); //히트당하면 끊긴다...
		GameManager.instance.uiManager.interingUI.On();
		float t = 0;
		float waitSec = p.Timefunc();
		while (waitSec * castModuleStat.Speed > t)
		{
			t += Time.deltaTime;
			GameManager.instance.uiManager.interingUI.SetGaugeValue(t / (waitSec * castModuleStat.Speed));
			yield return null;
		}
		p.onPrepComp?.Invoke(transform);
		ongoing = null;
		curName = null;
		GameManager.instance.uiManager.interingUI.Off();
		GameManager.instance.EnableCtrl(ControlModuleMode.Animated);
	}

	private void Update()
	{
		nowSkillSlot.Update();
		
		///
		if (Input.GetKeyDown(KeyCode.Y))
		{
			ConnectSkillDataTo(GameManager.instance.skillLoader.GetHumenSkill("BackKick"), SkillSlotInfo.One, PlayerForm.Magic);
		}
		if (Input.GetKeyDown(KeyCode.H))
		{
			ConnectSkillDataTo(GameManager.instance.skillLoader.GetHumenSkill("FireWall"), SkillSlotInfo.Two, PlayerForm.Magic);
		}
		if (Input.GetKeyDown(KeyCode.N))
		{
			ConnectSkillDataTo(GameManager.instance.skillLoader.GetHumenSkill("EnhanceIce"), SkillSlotInfo.Three, PlayerForm.Magic);
		}
		if (Input.GetKeyDown(KeyCode.RightShift))
		{
			ConnectSkillDataTo(GameManager.instance.skillLoader.GetHumenSkill("ChargeBowAttack"), SkillSlotInfo.Q, PlayerForm.Magic);
		}
		///
	}

	void UpdateClickSlots()
	{
		
		if (!nowSkillSlot[(int)SkillSlotInfo.LClick].IsEmpty && nowSkillSlot[(int)SkillSlotInfo.LClick].skInfo.useType != SkillUseType.Passive && !nowSkillSlot[(int)SkillSlotInfo.LClick].IsUsable)
			nowSkillSlot[(int)SkillSlotInfo.LClick].CurCooledTime += Time.deltaTime;
		nowSkillSlot[(int)SkillSlotInfo.LClick].skInfo?.UpdateStatus();

		if (!nowSkillSlot[(int)SkillSlotInfo.RClick].IsEmpty && nowSkillSlot[(int)SkillSlotInfo.RClick].skInfo.useType != SkillUseType.Passive && !nowSkillSlot[(int)SkillSlotInfo.RClick].IsUsable)
			nowSkillSlot[(int)SkillSlotInfo.RClick].CurCooledTime += Time.deltaTime;
		nowSkillSlot[(int)SkillSlotInfo.RClick].skInfo?.UpdateStatus();
	}

	public SkillRoot ConnectSkillDataTo(SkillRoot root, SkillSlotInfo to, PlayerForm st)
	{
		SkillRoot original = null;
		switch (st)
		{
			case PlayerForm.Magic:
				original = humenSkillSlot[((int)to)].skInfo;
				break;
			case PlayerForm.Yoho:
				original = yohoSkillSlot[((int)to)].skInfo;
				break;
			default:
				break;
		}
		if(original != null)
			original.Disoperate(GetActor());
		switch (st)
		{
			case PlayerForm.Magic:
				humenSkillSlot[((int)to)].Equip(root, GetActor());
				break;
			case PlayerForm.Yoho:
				yohoSkillSlot[((int)to)].Equip(root, GetActor());
				break;
			default:
				break;
		}
		Debug.Log($"스킬 장착 : {to}, {root.name}");
		
		return original;
	}

	public SkillRoot DisconnectSkillDataFrom(SkillSlotInfo from, PlayerForm st)
	{
		SkillRoot data = null;
		switch (st)
		{
			case PlayerForm.Magic:
				data = humenSkillSlot[((int)from)].skInfo;
				break;
			case PlayerForm.Yoho:
				data = yohoSkillSlot[((int)from)].skInfo;
				break;
			default:
				break;
		}
		if(data == null)
			return data;
		switch (st)
		{
			case PlayerForm.Magic:
				humenSkillSlot[((int)from)].UnEquip(GetActor());
				break;
			case PlayerForm.Yoho:
				yohoSkillSlot[((int)from)].UnEquip(GetActor());
				break;
			default:
				break;
		}
		Debug.Log($"스킬 해제 : {from}, {data.name}");
		return data;
	}

	internal void ResetSkillUse(SkillSlotInfo at)
	{
		DisoperateAt(at);
	}
	

	internal void ActualSkillOperate(SkillSlotInfo at)
	{
		Debug.Log(nowSkillSlot[((int)at)].skInfo.name);
		nowSkillSlot[((int)at)].skInfo.MyOperation(GetActor());
	}

	internal void ActualSkillDisoperate(SkillSlotInfo at)
	{
		nowSkillSlot[((int)at)].skInfo.MyDisoperation(GetActor());
	}

	internal void ActualSkillOperate(SkillSlotInfo at, int idx)
	{
		nowSkillSlot[((int)at)].skInfo.ActualOperateAt(GetActor(), idx);
	}

	internal void ActualSkillDisoperate(SkillSlotInfo at, int idx)
	{
		nowSkillSlot[((int)at)].skInfo.ActualDisoperateAt(GetActor(), idx);
	}

	internal void SetSkillUse(SkillSlotInfo at)
	{
		nowSkillSlot[((int)at)].skInfo.SetAnimations(GetActor(), at);
		UseSkillAt(at);
	}

	void UseSkillAt(SkillSlotInfo at)
	{
		if (nowSkillSlot[((int)at)].IsUsable)
		{
			switch (at)
			{
				case SkillSlotInfo.One:
					Cast(WOODSKILL);
					break;
				case SkillSlotInfo.Two:
					Cast(FIRESKILL);
					break;
				case SkillSlotInfo.Three:
					Cast(EARTHSKILL);
					break;
				case SkillSlotInfo.Q:
					Cast(METALSKILL);
					break;
				case SkillSlotInfo.E:
					Cast(WATERSKILL);
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
			case SkillSlotInfo.One:
				Cast(WOODSKILL+DISOPERATE);
				break;
			case SkillSlotInfo.Two:
				Cast(FIRESKILL + DISOPERATE);
				break;
			case SkillSlotInfo.Three:
				Cast(EARTHSKILL + DISOPERATE);
				break;
			case SkillSlotInfo.Q:
				Cast(METALSKILL + DISOPERATE);
				break;
			case SkillSlotInfo.E:
				Cast(WATERSKILL + DISOPERATE);
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
