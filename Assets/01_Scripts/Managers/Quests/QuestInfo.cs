using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class QuestInfo : ScriptableObject, System.IComparable
{
	[Header("이름")]
	public string questName;

	[Header("퀘스트 정보. 무조건 앞에서부터 판단함.")]
	public List<CompleteAtom> myInfo;

	[Header("보상 정보.")]
	public List<RewardAtom> rewardInfo;

	[Header("완료 가능 횟수. 0이하일 경우 무한 가능")]
	public int completableCount;

	public UnityEvent onCompleteAction;
	public UnityEvent onAssignedAction;
	public UnityEvent onDismissedAction;

	[Header("퀘스트 중 교체될 대사.")]
	public Dialogue questingDia;

	[Header("퀘스트 후 교체될 대사.")]
	public Dialogue questedDia;

	[Header("퀘스트 설명. 한 퀘스트당 하나")]
	public string descriptions;

	[Header("퀘스트 종류.")]
	public QuestType type;

	internal int curCompletedAmount;
	internal Character giver;
	internal bool assigned = false;

	internal bool rewarding = false;

	public bool IsDeprived => completableCount >= 0 && curCompletedAmount >= completableCount;

	bool? everSince = null;
	Dictionary<string, GameObject> relatedObjects;
	public bool NeedCheck
	{
		get
		{
			if(everSince == null)
			{
				everSince = myInfo.Find(item => item.everSince) != null;
			}
			return !IsDeprived && (assigned || (bool)everSince);
		}
	}

	public bool IsPendingCompletion
	{
		get
		{
			return BlankExamine(); //##############
		}
	}

	protected virtual void OnEnable()
	{
		if (myInfo == null)
			return;
		ResetQuestCall();
		ResetQuestStartTime();
		curCompletedAmount = 0;
		assigned = false;

		relatedObjects = new Dictionary<string, GameObject>();

		for (int i = 0; i < rewardInfo.Count; i++)
		{
			if(rewardInfo[i].rewardType == RewardType.EnableObject || rewardInfo[i].rewardType == RewardType.DisableObject)
			{
				relatedObjects.Add(rewardInfo[i].parameter, GameObject.Find(rewardInfo[i].parameter));
			}
		}
	}

	public void ResetQuestStartTime(CompletionAct cond = CompletionAct.None)
	{
		
		for (int i = 0; i < myInfo.Count; i++)
		{
			if(cond == CompletionAct.None || myInfo[i].objective == cond)
			{
				myInfo[i].OnResetTime();
			}
		}
	}

	public void ResetQuestCall(CompletionAct cond = CompletionAct.None)
	{
		for (int i = 0; i < myInfo.Count; i++)
		{
			if (cond == CompletionAct.None || myInfo[i].objective == cond)
			{
				myInfo[i].OnResetCall();

			}
		}
	}
	public QuestInfo()
	{
		questName = "";
		myInfo = new List<CompleteAtom>();
		rewardInfo = new List<RewardAtom>();
		completableCount = 1;
		assigned = false;
		everSince = null;
		rewarding = false;
	}

	public QuestInfo(QuestInfo copy)
	{
		if(copy != null)
		{
			questName = copy.questName;
			myInfo = new List<CompleteAtom>(copy.myInfo);
			rewardInfo = new List<RewardAtom>(copy.rewardInfo);
			completableCount = copy.completableCount;
			assigned = copy.assigned;
			everSince = copy.everSince;
			rewarding = true;
		}
		else
		{
			questName = "";
			myInfo = new List<CompleteAtom>();
			rewardInfo = new List<RewardAtom>();
			completableCount = 1;
			assigned = false;
			everSince = null;
			rewarding = false;
		}
	}

	//public bool Notify(CompletionAct data, string parameter, int amt)
	//{
	//	bool res = false;
	//	for (int i = 0; i < myInfo.Count; i++)
	//	{
	//		res |= myInfo[i].OnNotification(data, parameter, amt);
	//	}
	//	return res;
	//}

	public bool ExamineCompleteStatus(CompletionAct type, string prm, int amt)
	{
		bool compStat = false;

		int head = 0;
		bool valid = true;
		AfterComplete prevConnection = AfterComplete.FINAL;
		while (valid)
		{
			if(myInfo[head].examineStartTime == null)
			{
				myInfo[head].examineStartTime = (int)Time.time;
			}

			bool res = myInfo[head].OnNotification(type, prm, amt);
			Debug.Log(myInfo[head].objective + " : " + res);
			if(head == 0)
			{
				compStat = res;
			}

			switch (prevConnection)
			{
				case AfterComplete.AND:
					compStat &= res;
					break;
				case AfterComplete.OR:
					compStat |= res;
					break;
				case AfterComplete.AFTER:
					if (!compStat)
					{
						myInfo[head].Freeze();
						Debug.Log(QuestManager.ToStringKorean(myInfo[head - 1].objective) + " 가 아직 완료되지 않아서 " + QuestManager.ToStringKorean(myInfo[head].objective) + " 동결.");
						return false;
					}
					else
					{
						myInfo[head].UnFreeze();
						Debug.Log(QuestManager.ToStringKorean(myInfo[head - 1].objective) + " 가 완료되어 " + QuestManager.ToStringKorean(myInfo[head].objective) + " 동결 해제.");
						compStat &= res;
					}
					break;
				default:
					break;
			}

			prevConnection = myInfo[head].afterAct;

			if(prevConnection == AfterComplete.FINAL)
				valid = false;
			++head;
		}

		return compStat;
	}

	bool BlankExamine()
	{
		bool compStat = false;

		int head = 0;
		bool valid = true;
		AfterComplete prevConnection = AfterComplete.FINAL;
		while (valid)
		{

			
			bool res = myInfo[head].isCompleted;

			if (head == 0)
			{
				compStat = res;
			}

			switch (prevConnection)
			{
				case AfterComplete.AND:
					compStat &= res;
					break;
				case AfterComplete.OR:
					compStat |= res;
					break;
				case AfterComplete.AFTER:
					compStat &= res;
					break;
				default:
					break;
			}

			

			prevConnection = myInfo[head].afterAct;

			if (prevConnection == AfterComplete.FINAL)
				valid = false;
			
			++head;

			if (myInfo[head].afterAct == AfterComplete.FINAL)
			{
				return compStat;
			}
		}

		return compStat;
	}

	public void GiveReward()
	{
		rewarding = true;
		GameManager.instance.qManager.InvokeOnChanged(CompletionAct.ClearQuest, questName);

		for (int i = 0; i < myInfo.Count; i++)
		{
			if(myInfo[i].objective == CompletionAct.GetItem || myInfo[i].objective == CompletionAct.HaveItem)
			{
				if(myInfo[i].itemMode == ItemHandleMode.Remove)
				{
					GameManager.instance.pinven.RemoveItem(Item.GetItem <Item> (myInfo[i].parameter), myInfo[i].repeatCount);
				}
			}
		}

		for (int i = 0; i < rewardInfo.Count; i++)
		{
			switch (rewardInfo[i].rewardType)
			{
				case RewardType.Exp:
					{
						Debug.Log($"경험치 {rewardInfo[i].parameter} 제공함");
						GameManager.instance.pinven.AddExp(rewardInfo[i].amount);
					}
					break;
				case RewardType.Skill:
					{
						Debug.Log($"스킬 {rewardInfo[i].parameter} 제공함");
						//?????????
					}
					break;
				case RewardType.Item:
					{
						Debug.Log($"아이템 {rewardInfo[i].parameter} 제공함");
						GameManager.instance.pinven.AddItem(Item.GetItem <Item> (rewardInfo[i].parameter), rewardInfo[i].amount);
					}
					break;
				case RewardType.HealWhite:
					{
						//int amt = int.Parse(rewardInfo[i].parameter);
						GameManager.instance.pActor.life.DamageYY(0, -rewardInfo[i].amount, DamageType.NoHit);
					}
					break;
				case RewardType.HealBlack:
					{
						//int amt = int.Parse(rewardInfo[i].parameter);
						GameManager.instance.pActor.life.DamageYY(-rewardInfo[i].amount, 0, DamageType.NoHit);
					}
					break;
				case RewardType.Quest:
					{
						QuestManager.AssignQuest(rewardInfo[i].parameter);
					}
					break;
				case RewardType.EnableObject:
					{
						relatedObjects[rewardInfo[i].parameter].SetActive(true);
					}
					break;
				case RewardType.DisableObject:
					{
						relatedObjects[rewardInfo[i].parameter].SetActive(false);
					}
					break;
				default:
					break;
			}
		}

		if(giver != null && questedDia != null)
		{
			giver.SetDialogue(questedDia);
		}
		
		rewarding = false;
		onCompleteAction?.Invoke();
	}

	public override bool Equals(object other)
	{
		return other is QuestInfo info && info.questName == this.questName;
	}

	public override string ToString()
	{
		return questName;
	}

	public override int GetHashCode()
	{
		return System.HashCode.Combine(questName);
	}

	public int CompareTo(object obj)
	{
		if(obj is QuestInfo q)
		{
			return name.CompareTo(q.name);
		}
		return 0;
	}
}
