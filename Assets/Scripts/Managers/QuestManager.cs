using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Quests
{
	None = -1,
	MOVE,
	
	MAKEJUMPMEDICINE,

	HUNTBEAR,

	MAX
}

public class QuestManager
{
    public List<string> questDescOrder;
	public Quests curIdx = Quests.None;

	public void NextQuest()
	{
		if(curIdx != Quests.MAX)
		{
			curIdx += 1;
			GameManager.instance.uiManager.questUI.SetText(questDescOrder[((int)curIdx)]);
			GameManager.instance.uiManager.questUI.On();
		}
		else
		{
			GameManager.instance.uiManager.questUI.Off();
		}
		
	}

	public void NextIf(Quests info)
	{
		if(curIdx == info)
		{
			NextQuest();
		}
	}

	public QuestManager(params string[] questDescs)
	{
		questDescOrder = new List<string>(questDescs);
	}
}
