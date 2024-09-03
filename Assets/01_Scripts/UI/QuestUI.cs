using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestUI : MonoBehaviour, IOpenableWindowUI
{
	QuestDetailUI questInfo;
	QuestListUI questList;

	public bool isOverlay { get; set; } = false;

	public const string CONDUINAME = "CompletionAtomBgnd";
	public const string REWUINAME = "RewAtomBgnd";
	public const string SEPARATORNAME = "QuestSeparator";
	public const string INFOUINAME = "QuestInfoBgnd";

	public void OnClose()
	{
		questInfo.SetInfoEmpty();

	}

	public void OnOpen()
	{
		if(GameManager.instance.qManager.currentAbleQuest.Count > 0)
		{
			RefreshQuestListCompletely();
			questInfo.ShowInfoOf(GameManager.instance.qManager.currentAbleQuest[0]);
		}
		else
		{
			questInfo.SetInfoEmpty();
		}
	}

	public void Refresh()
	{
		questInfo.RefreshCompInfo();
		questList.RefreshQuestUIState();
	}

	public void RefreshQuestListCompletely()
	{
		questList.RefreshAvailQuests();
	}

	public void WhileOpening()
	{

	}

	void Awake()
    {
        questInfo = GetComponentInChildren<QuestDetailUI>();
		questList = GetComponentInChildren<QuestListUI>();
    }

	public void ShowInfo(QuestInfo inf)
	{
		questInfo.ShowInfoOf(inf);
	}
}
