using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DialogueLoader
{
	static Dictionary<string, Dialogue> conversationDialoguePair;
	static bool inited = false;

//	public static Dictionary<string, Dialogue> ConversationDialoguePair
//	{
//		get
//		{

////			if (!inited)
////			{
////				conversationDialoguePair = new Dictionary<string, Dialogue>();
////				string[] res = (Directory.GetFiles($"{QuestManager.ASSETPATH}{Dialogue.DIALOGUEPATH}", "*.meta", SearchOption.TopDirectoryOnly));
////				int pLeng = $"{QuestManager.ASSETPATH}{Dialogue.DIALOGUEPATH}".Length;
////				foreach (var item in res)
////				{
////					conversationDialoguePair.Add(item.Substring(pLeng).Split('.')[0], Resources.LoadAll<Dialogue>($"{Dialogue.DIALOGUEPATH}{item.Substring(pLeng).Split('.')[0]}/")[0]);
////				}
////#if !UNITY_EDITOR
////				inited = true;
////#endif
////			}

//			return conversationDialoguePair;
//		}
//	}
}