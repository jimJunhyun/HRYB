using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 1. 필요로 하는 물건 / 행동 과 수량
/// 2. 완수 후 보상 제공 시점
/// 
/// 커스텀 윈도우에서 만들어줄예정.
/// </summary> 

public enum AfterComplete
{

	AND,
	OR,
	AFTER,

	FINAL
}

/// <summary>
/// 어떤 아이템을 얻어서 퀘스트 주인에게 전달하는 경우,
/// 전달하는 과정까지 하나의 퀘스트로 봅니다.
/// </summary>
public enum CompletionAct 
{
	None = -1,

	GetItem, //획득시 달성으로 취급.  V
	HaveItem, //소지한 동안 달성으로 취급.  V (비활성화 상태의 퀘스트에도 검사해주어야하는가..)
	LoseItem, //잃을 경우 달성으로 취급.  V
	UseItem, //사용할 경우 달성으로 취급.  V

	DefeatTarget, //해당 이름의 적을 처치  V

	CountSecond, //n초후 달성으로 취급, 사망시 초기화됨.  V
	MoveTo, //지점으로 이동. 목표지점은 Trigger이며 레이어는 25번이어야 함.  V
	InteractWith, //대상과 상호작용. 대상에는 반드시  V

	//n레벨 이하/동일/이상 일 경우 달성으로 취급. 레벨이 정해지면 더 자세히 나올듯.
	//레벨 오를 때 이전 레벨의 BelowLevel -1, 이전 레벨의 Eq - 1, 현재 레벨의 Eq + 1, 현재 레벨의 Above + 1
	BelowLevel, 
	EqualLevel,
	AboveLevel,

	Callback,

	GoNear, //가까이 간 순간 달성    V
	RemainNear, //가까이 있는 동안 달성    V

	ClearQuest, //특정 퀘스트 클리어시 달성. V
	LearnSkill, //스킬 배우면 달성   V

	//더 있으면 추가.
}


public enum ItemHandleMode
{
	Remove,
	NoRemove,
}


public enum Comparer
{
	More,
	Equal,
	Less,

}

public enum DefeatEnemyMode
{
	None,
	Form,
	Skill,
	Element,
	OverDamage,
	DamageType,
	StatusEffect,
	SelfStatusEffect,
}

public enum RewardType
{
	Exp,
	Skill,
	Item,
	HealWhite,
	HealBlack,
	Quest,
	EnableObject,
	DisableObject,
	PlayTimeline,
}

public enum QuestType
{
	Main,
	Sub,

}

[Serializable]
public class DetailCondition
{
	public DetailParameter paramType;
	public bool inverted;
	public Comparer comp;
	public float value;

	public AfterComplete after;

	public bool ExamineStatus(YinyangItem item)
	{
		float v = item.detailParams[paramType];
		bool r;
		switch (comp)
		{
			case Comparer.More:
				r = v > value;
				break;
			case Comparer.Equal:
				r = v == value;
				break;
			case Comparer.Less:
				r = v < value;
				break;
			default:
				r = false;
				break;
		}
		if(inverted)
			r = !r;
		return r;
	}
}

[Serializable]
public class DetailConditionList : List<DetailCondition>
{
	public bool ExamineStatus(YinyangItem item)
	{
		if(this.Count <= 0)
			return true;
		int head = 0;
		bool res = true;
		bool valid = true;
		AfterComplete prev = AfterComplete.FINAL;
		while (valid)
		{
			if(prev == AfterComplete.FINAL)
				res = this[head].ExamineStatus(item);
			else
			{

				switch (prev)
				{
					case AfterComplete.AND:
						res &= this[head].ExamineStatus(item);
						break;
					case AfterComplete.OR:
						res |= this[head].ExamineStatus(item);
						break;
					default:
						res &= this[head].ExamineStatus(item);
						break;
				}

			}
			prev = this[head].after;

			if(prev == AfterComplete.FINAL)
				valid = false;
			head += 1;
		}
		return res;
	}
}


[System.Serializable]
public class CompleteAtom
{
	//필수
	public CompletionAct objective;
	[Header("목적의 대상")]
	public string parameter;
	[Header("달성에 필요한 횟수")]
	public int repeatCount;

	[Header("이후 액션과 어떻게 연결될 것인가?")]
	public AfterComplete afterAct;
	[Header("체크될 경우, 위 조건이 달성되지 않은 동안 완료로 취급.")]
	public bool inverted;

	[Header("체크될 경우, 퀘스트가 활성화되지 않아도 카운트함. (히든퀘/누적형)")]
	public bool everSince;

	[Header("체크될 경우, 조건이 보여짐.")]
	public bool isVisibleCondition;

	//비필수
	[Header("아이템을 제거할 것인가?")]
	public ItemHandleMode itemMode;

	[Header("아이템 세부 달성 조건")]
	public bool itemRequirement;
	public DetailConditionList conds;

	[Header("어떻게 적을 잡을 것인가?")]
	public DefeatEnemyMode defeatMode;
	public string defeatParameter;

	public bool isCompleted => curRepeatCount >= repeatCount;

	internal int curRepeatCount;

	internal int? examineStartTime;

	internal bool conditionFrozen = false;

	public bool OnNotification(CompletionAct data, string parameter, int amt)
	{
		if(conditionFrozen)
			return false;

		if(objective == data)
		{ 
			switch (data)
			{
				case CompletionAct.DefeatTarget:
				{
					if (defeatMode != DefeatEnemyMode.None)
					{

						string enemyDefeatCapture = GameManager.instance.DoCapture();
						if (GameManager.instance.Decode(enemyDefeatCapture)[defeatMode].Contains(defeatParameter)) //Decode로 변환.
						{
							if(this.parameter == parameter || this.parameter == "")
							{
								curRepeatCount += amt;
							}
						}
					}
					else
					{
						if (this.parameter == parameter || this.parameter == "")
						{
							curRepeatCount += amt;
						}
					}
					
				}
					break;
				case CompletionAct.BelowLevel:
					try
					{
						if (int.Parse(parameter) < int.Parse(this.parameter))
						{
							curRepeatCount += amt;
						}
					}
					catch
					{
						throw new UnityException("숫자쓰는데에 글자쓰지 마라");
					}
					break;
				case CompletionAct.CountSecond:
					try
					{
						if (int.Parse(parameter) > int.Parse(this.parameter) + examineStartTime)
						{ 
							curRepeatCount += amt;
						}
					}
					catch
					{
						throw new UnityException("숫자쓰는데에 글자쓰지 마라");
					}
					break;
				case CompletionAct.AboveLevel:
					try
					{
						if (int.Parse(parameter) > int.Parse(this.parameter))
						{
							curRepeatCount += amt;
						}
					}
					catch
					{
						throw new UnityException("숫자쓰는데에 글자쓰지 마라");
					}
					break;
				case CompletionAct.Callback:
					throw new NotImplementedException("콜백은 아직 적용되지 않았습니다.");
				case CompletionAct.GetItem:
				case CompletionAct.HaveItem:
				case CompletionAct.LoseItem:
				case CompletionAct.UseItem:
					if(itemRequirement)
					{
						if (conds.ExamineStatus(Item.GetItem<YinyangItem>(parameter)))
						{
							curRepeatCount += amt;
						}
					}
					else
					{
						if (parameter == this.parameter)
						{
							curRepeatCount += amt;
						}
					}
					break;
				default:
					if(parameter == this.parameter)
					{
						curRepeatCount += amt;
					}
					break;
			}
		}
		Debug.Log("검사됨 : " + objective);
		return isCompleted;
	}

	public void Freeze()
	{
		conditionFrozen = true;
	}

	public void UnFreeze()
	{
		conditionFrozen = false;
	}

	public void OnResetCall()
	{
		curRepeatCount = 0;
		conditionFrozen = false;
		Debug.Log("초기화됨 : " + objective);
	}

	public void OnResetTime()
	{
		examineStartTime = null;
	}

	public CompleteAtom()
	{
		curRepeatCount = 0;
		examineStartTime = null;
	}
}

[System.Serializable]
public class RewardAtom
{
	public RewardType rewardType;
	public string parameter;
	public int amount;
}



public class QuestManager
{

	public static Dictionary<string, QuestInfo> nameQuestPair;

	[Header("동시에 완료된다면 먼저 받은 퀘스트부터 완료 판정이 뜰 것임.")]
	public List<QuestInfo> currentAbleQuest = new List<QuestInfo>();

	static List<QuestInfo> allQuests = new List<QuestInfo>();

	//UnityAction<CompletionAct, string, int> invokers;

	bool inited = false;

	public const string QUESTINFOPATH = "Quests/AllQuests/";
	public const string ASSETPATH = "Assets/Resources/";


	//이제 이놈을 행동하는 데마다 하나씩 꼽아주면 됨.
	public void InvokeOnChanged(CompletionAct type, string prm, int amt = 1)
	{
		CheckQuests(type, prm, amt);
		GameManager.instance.uiManager.UpdateQuestUI();
	}

	public static void AssignQuest(string name, Character giver = null)
	{
		
		if (nameQuestPair != null)
		{
			if (!nameQuestPair.ContainsKey(name))
				return;
			QuestInfo data = nameQuestPair[name];
			GameManager.instance.qManager.currentAbleQuest.Add(data);
			if (giver != null && data.questingDia != null)
			{
				giver.SetDialogue(data.questingDia);
				data.giver = giver;
			}
			nameQuestPair[name].assigned = true;
			nameQuestPair[name].onAssignedAction?.Invoke();
		}
	}

	public static void AssignQuest(int idx) //파일내 순서(ABC)로 부여. 아마 안쓸듯?
	{
		if (allQuests != null)
		{
			GameManager.instance.qManager.currentAbleQuest.Add(allQuests[idx]);
			allQuests[idx].assigned = true;
			allQuests[idx].onAssignedAction?.Invoke();
		}
	}

	public static void DismissQuest(string name)
	{
		
		if (nameQuestPair != null)
		{
			if (!nameQuestPair.ContainsKey(name))
				return;
			QuestInfo inf = nameQuestPair[name];
			if (GameManager.instance.qManager.currentAbleQuest.Contains(inf))
			{
				GameManager.instance.qManager.currentAbleQuest.Remove(inf);
				if(inf.questingDia != null && inf.giver != null)
				{
					inf.giver.self.talk.ResetStatus();
				}
				inf.assigned = false;
				inf.onDismissedAction?.Invoke();
			}

				
		}
	}

	public static void DismissQuest(int idx)//파일내 순서(ABC)로 취소. 아마 안쓸듯?
	{
		if (allQuests != null)
		{
			GameManager.instance.qManager.currentAbleQuest.Remove(allQuests[idx]);
			allQuests[idx].assigned = false;
			allQuests[idx].onDismissedAction?.Invoke();
		}
	}

	public void CheckQuests(CompletionAct type, string prm, int amt)
	{
		List<QuestInfo> completeds = new List<QuestInfo>();
		
		for (int i = 0; i < allQuests.Count; i++)
		{
			if (!allQuests[i].rewarding && allQuests[i].NeedCheck && allQuests[i].ExamineCompleteStatus(type, prm, amt))
			{
				if (currentAbleQuest.Contains(allQuests[i]))
				{
					completeds.Add(allQuests[i]);
				}
			}
		}

		for (int i = 0; i < completeds.Count; i++)
		{

			completeds[i].GiveReward();
			completeds[i].curCompletedAmount += 1;
			if(completeds[i].IsDeprived && completeds[i].assigned)
			{
				currentAbleQuest.Remove(completeds[i]);
			}
		}
		completeds.Clear();
	}

	public QuestManager()
	{
		if (!inited)
		{
			nameQuestPair = new Dictionary<string, QuestInfo>();
			allQuests = Resources.LoadAll<QuestInfo>(QUESTINFOPATH).ToList();
			for (int i = 0; i < allQuests.Count; i++)
			{
				allQuests[i].Init();
				nameQuestPair.Add((allQuests[i].questName), allQuests[i]);
			}

			inited = true;

			currentAbleQuest = new List<QuestInfo>();
		}
	}




	


	public static string ToStringKorean(AfterComplete act)
	{
		switch (act)
		{
			case AfterComplete.AND:
				return ": 그리고";
			case AfterComplete.OR:
				return ": 또는";
			case AfterComplete.AFTER:
				return ": 그 다음";
			case AfterComplete.FINAL:
				return "로 종료.";
			default:
				Debug.LogWarning($"{act} 상태에 대한 한글 번역이 제공되지 않습니다.");
				return act.ToString();
		}
	}
	public static string ToStringKorean(CompletionAct act)
	{
		switch (act)
		{
			case CompletionAct.None:
				return "기본";
			case CompletionAct.GetItem:
				return "아이템 획득하기. 이름 : ";
			case CompletionAct.HaveItem:
				return "아이템 소지하기. 이름 : ";
			case CompletionAct.LoseItem:
				return "아이템 잃기. 이름 : ";
			case CompletionAct.UseItem:
				return "아이템 사용하기. 이름 : ";
			case CompletionAct.DefeatTarget:
				return "적 처치하기. 이름 : ";
			case CompletionAct.CountSecond:
				return "n초 세기. n = ";
			case CompletionAct.MoveTo:
				return "지점으로 이동하기. 트리거 오브젝트 이름 : ";
			case CompletionAct.InteractWith:
				return "상호작용하기. 상호작용 가능 오브젝트 이름 : ";
			case CompletionAct.BelowLevel:
				return "n레벨 이하. n = ";
			case CompletionAct.EqualLevel:
				return "n레벨 유지하기. n = ";
			case CompletionAct.AboveLevel:
				return "n레벨 이상. n = ";
			case CompletionAct.Callback:
				return "스크립트에서 진행됨.";
			case CompletionAct.GoNear:
				return "가까이 접근하기. 대상 : ";
			case CompletionAct.RemainNear:
				return "근처에 머무르기. 대상 : ";
			case CompletionAct.ClearQuest:
				return "퀘스트 클리어하기. 식별명 : ";
			case CompletionAct.LearnSkill:
				return "스킬 배우기. 스킬명 : ";
			default:
				Debug.LogWarning($"{act} 상태에 대한 한글 번역이 제공되지 않습니다.");
				return act.ToString();
		}
	}
	public static string ToStringKorean(RewardType act)
	{
		switch (act)
		{
			case RewardType.Exp:
				return "경험치 : ";
			case RewardType.Skill:
				return "스킬 이름 : ";
			case RewardType.Item:
				return "아이템 이름 : ";
			case RewardType.HealWhite:
				return "양(하얀거) 회복 : ";
			case RewardType.HealBlack:
				return "음(검은거) 회복 : ";
			case RewardType.Quest:
				return "퀘스트 제공(식별명) : ";
			case RewardType.EnableObject:
				return "오브젝트 활성화 : ";
			case RewardType.DisableObject:
				return "오브젝트 비활성화 : ";
			case RewardType.PlayTimeline:
				return "타임라인 재생 : ";
			default:
				Debug.LogWarning($"{act} 상태에 대한 한글 번역이 제공되지 않습니다.");
				return act.ToString();
		}
	}
	public static string ToStringKorean(ItemHandleMode act)
	{
		switch (act)
		{
			case ItemHandleMode.Remove:
				return "달성될 경우 아이템을 제거함.";
			case ItemHandleMode.NoRemove:
				return "달성되어도 아이템을 제거하지 않음.";
			default:
				Debug.LogWarning($"{act} 상태에 대한 한글 번역이 제공되지 않습니다.");
				return act.ToString();
		}
	}

	public static string ToStringKorean(Comparer act)
	{
		switch (act)
		{
			case Comparer.More:
				return " > ";
			case Comparer.Equal:
				return " == ";
			case Comparer.Less:
				return " < ";
			default:
				Debug.LogWarning($"{act} 상태에 대한 한글 번역이 제공되지 않습니다.");
				return act.ToString();
		}
	}
	public static string ToStringKorean(DefeatEnemyMode act)
	{
		switch (act)
		{
			case DefeatEnemyMode.Form:
				return " 상태에서 처치하기.";
			case DefeatEnemyMode.Skill:
				return " 스킬로 처치하기.";
			case DefeatEnemyMode.Element:
				return " 속성으로 처치하기.";
			case DefeatEnemyMode.OverDamage:
				return " 이상 피해로 처치하기.";
			case DefeatEnemyMode.DamageType:
				return " 피해로 처치하기. (지속, 도트, 일반)";
			case DefeatEnemyMode.StatusEffect:
				return " 상태를 부여한 상태로 처치하기.";
			case DefeatEnemyMode.SelfStatusEffect:
				return " 상태를 가진 상태로 처치하기.";
			case DefeatEnemyMode.None:
				return "조건 없음.";
			default:
				Debug.LogWarning($"{act} 상태에 대한 한글 번역이 제공되지 않습니다.");
				return act.ToString();
		}
	}

	public static string ToStringKorean(DetailParameter act)
	{
		switch (act)
		{
			case DetailParameter.Sweet:
				return "단맛";
			case DetailParameter.Sour:
				return "신맛";
			case DetailParameter.Bitter:
				return "쓴맛";
			case DetailParameter.Salty:
				return "짠맛";
			case DetailParameter.Spicy:
				return "매운맛";
			case DetailParameter.Moist:
				return "수분";
			case DetailParameter.Poison:
				return "독성";
			default:
				Debug.LogWarning($"{act} 상태에 대한 한글 번역이 제공되지 않습니다.");
				return act.ToString();
		}
	}

}