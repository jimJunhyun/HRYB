using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestDetailUI : MonoBehaviour
{
	Transform condContent;
	Transform rewContent;

	QuestInfo curShown;

	TextMeshProUGUI questName;
	//TextMeshProUGUI giver;
	TextMeshProUGUI description;

	List<QuestConditionUI> conds= new List<QuestConditionUI>();
	List<QuestRewardUI> rews= new List<QuestRewardUI>();

	public void ShowInfoOf(QuestInfo inf)
	{
		On();
		if(inf == null)
		{
			//OffCompletely();
			SetInfoEmpty();
			return;
		}
		curShown = inf;

		System.Text.StringBuilder sb;
		bool usingGlobal = GameManager.GetGlobalSB(out sb);

		questName.text = inf.questName;
		//sb.Append("제공자 : <size=80%>");
		//sb.Append(inf.giver.subName);
		//sb.Append(" <size=100%>");
		//sb.Append(inf.giver.baseName);
		//giver.text = sb.ToString();

		sb.Clear();
		sb.Append("<size=80%>");
		sb.Append(inf.descriptions);
		sb.Append("<size=100%>");
		description.text = sb.ToString();

		GameManager.ReturnGlobalSB(usingGlobal);

		for (int i = 0; i < conds.Count; i++)
		{
			PoolManager.ReturnObject(conds[i].gameObject);
		}
		for (int i = 0; i < rews.Count; i++)
		{
			PoolManager.ReturnObject(rews[i].gameObject);
		}
		conds.Clear();
		rews.Clear();

		for (int i = 0; i < inf.myInfo.Count; i++)
		{
			if (inf.myInfo[i].isVisibleCondition)
			{
				QuestConditionUI cond = PoolManager.GetObject(QuestUI.CONDUINAME, condContent).GetComponent<QuestConditionUI>();
				cond.ShowCond(inf.myInfo[i]);
				conds.Add(cond);
			}
		}


		for (int i = 0; i < inf.rewardInfo.Count; i++)
		{
			if(inf.rewardInfo[i].rewardType != RewardType.Quest && inf.rewardInfo[i].rewardType != RewardType.HealWhite && inf.rewardInfo[i].rewardType != RewardType.HealBlack)
			{
				QuestRewardUI rew = PoolManager.GetObject(QuestUI.REWUINAME, rewContent).GetComponent<QuestRewardUI>();
				rew.ShowInfo(inf.rewardInfo[i]);
				rews.Add(rew);
			}
		}
	}

	public void SetInfoEmpty()
	{
		questName.text = "";
		description.text = "선택한 퀘스트 정보를 표시합니다.";
		for (int i = 0; i < conds.Count; i++)
		{
			PoolManager.ReturnObject(conds[i].gameObject);
		}
		for (int i = 0; i < rews.Count; i++)
		{
			PoolManager.ReturnObject(rews[i].gameObject);
		}
		conds.Clear();
		rews.Clear();

		curShown = null;
	}

	public void RefreshCompInfo()
	{
		//for (int i = 0; i < conds.Count; i++)
		//{
		//	conds[i].RefreshCount();
		//}
	}

	public void OffCompletely()
	{
		for (int i = 0; i < conds.Count; i++)
		{
			PoolManager.ReturnObject(conds[i].gameObject);
		}
		for (int i = 0; i < rews.Count; i++)
		{
			PoolManager.ReturnObject(rews[i].gameObject);

		}
		conds.Clear();
		rews.Clear();
		gameObject.SetActive(false);
	}

	public void On()
	{
		if (condContent == null)
		{
			condContent = transform.Find("QuestInfoScroll/Viewport/QuestConditionList/Viewport/QuestConditionContent");
		}
		if (rewContent == null)
		{
			rewContent = transform.Find("QuestInfoScroll/Viewport/QuestRewardList/Viewport/QuestRewardContent");
		}

		if (questName == null)
		{
			questName = transform.Find("QuestInfoScroll/Viewport/QuestName").GetComponent<TextMeshProUGUI>();
		}
		//if(giver == null)
		//{
		//	giver = transform.Find("QuestInfoScroll/Viewport/Content/NameText").GetComponent<TextMeshProUGUI>();
		//}
		if (description == null)
		{
			description = transform.Find("QuestInfoScroll/Viewport/DescText").GetComponent<TextMeshProUGUI>();
		}

		gameObject.SetActive(true);
	}
}
