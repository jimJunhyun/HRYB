using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestListUI : MonoBehaviour
{
	Transform content;
	List<QuestInfoUI> infos = new List<QuestInfoUI>();
	//List<QuestInfoSeparatorUI> seps = new List<QuestInfoSeparatorUI>();
    public void RefreshAvailQuests()
	{
		if(content == null)
		{
			content = transform.Find("QuestList/Viewport/QuestListContent");
		}

		for (int i = 0; i < infos.Count; i++)
		{
			PoolManager.ReturnObject(infos[i].gameObject);
		}
		//for (int i = 0; i < seps.Count; i++)
		//{
		//	PoolManager.ReturnObject(seps[i].gameObject);
		//}
		infos.Clear();
		//seps.Clear();

		Dictionary<QuestType, SortedSet<QuestInfo>> typeQuestPair = new Dictionary<QuestType, SortedSet<QuestInfo>>();
		for (int i = ((int)QuestType.Main); i <= ((int)QuestType.Sub); i++)
		{
			typeQuestPair.Add((QuestType)i, new SortedSet<QuestInfo>());
		}
		for (int i = 0; i < GameManager.instance.qManager.currentAbleQuest.Count; i++)
		{
			typeQuestPair[GameManager.instance.qManager.currentAbleQuest[i].type].Add(GameManager.instance.qManager.currentAbleQuest[i]);
		}
		foreach (var item in typeQuestPair)
		{
			//QuestInfoSeparatorUI sep = PoolManager.GetObject(QuestUI.SEPARATORNAME, content).GetComponent<QuestInfoSeparatorUI>();
			//sep.ShowInfo(item.Key);
			//seps.Add(sep);
			foreach (var quest in item.Value)
			{
				QuestInfoUI inf = PoolManager.GetObject(QuestUI.INFOUINAME, content).GetComponent<QuestInfoUI>();
				inf.ShowInfo(quest);
				infos.Add(inf);
			}
		}
	}

	public void RefreshQuestUIState()
	{
		for (int i = 0; i < infos.Count; i++)
		{
			infos[i].RefreshNum();
		}
	}
}
