using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(menuName = "캐릭터 데이터")]

//이게 없으면 말을 못할듯?
public class Character : ScriptableObject
{
	internal Actor self;

	public string FullName
	{
		get => $"{subName} {baseName}";
	} 

    public string baseName;
	public string subName;

	public Dialogue initDia;

	bool? q = null;
	public bool Questing
	{
		get
		{
			if(q == null)
			{
				q = false;
				Dialogue nxt = dia;
				while (true)
				{
					if(nxt is ChoiceDialogue cd)
					{
						nxt = cd.nexts[0];
					}
					else{
						nxt = nxt.next;
					}
					if(nxt == null)
					{
						Debug.Log("NOQUESTING");
						break;
					}
					if((nxt as QuestDialogue) != null)
					{
						Debug.Log("ZNPTMEND");
						q = true;
						break;
					}

				}
			}
			return (bool)q;
		}
	}

	public QuestInfo latestQuest;

	Dialogue dia;

	private void Awake()
	{
		q = null;
		dia = initDia.Copy();
	}

	public void SetDialogue(Dialogue dia)
	{
		q = null;
		this.dia = dia;
	}

	public void ResetDialogue()
	{
		q = null;
		dia = initDia.Copy();
	}
	
	public void OnTalk()
	{
		GameManager.instance.uiManager.dialogueUI.talker = this;
		//GameManager.instance.uiManager.dialogueUI.talker = this;
		Debug.Log($"{FullName}이 말하는 중 ...");
		dia.OnShown(this);
	}
}
