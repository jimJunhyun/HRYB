using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Threading;

public class ParseDialogue : Editor
{
	const int CONVTYPE = 1;
	const int DIATEXT = 2;
	const int NPCNAME = 3;
	const int QUESTNAME = 5;
	const int REWARDITEM = 6;
	const int NEXTDIA = 8;
	const int SWAPDIA = 13;

	[MenuItem("대화/대화 가져오기")]
	public static void DoParse()
	{
		CreateDialogue();
		
	}

	static void CreateDialogue() //async로 바꾸는게?
	{
		SheetParser data = new SheetParser("https://docs.google.com/spreadsheets/d/1U_d85oU7k3LJym1HeIO90zeiGZhk2D-k8w3PR9CgzaQ/export?format=tsv&gid=649135811&range=B3:O", "B", "O", true, false);
		data.onCompleted += ParseDia;
	}

	static void ParseDia(SheetParser ps)
	{
		

		string path = $"{QuestManager.ASSETPATH}{DialogueFlowViewer.DIALOGUEPATH}";

		//DirectoryInfo di = new DirectoryInfo(path);

		//foreach (FileInfo file in di.GetFiles())
		//{
		//	file.Delete();
		//}
		//foreach (DirectoryInfo dir in di.GetDirectories())
		//{
		//	dir.Delete(true);
		//}

		Dictionary<string, Dictionary<string, List<Dialogue>>> npcDatas = new Dictionary<string, Dictionary<string, List<Dialogue>>>();

		for (int i = ps.cardinality - 1; i >= 0; --i)
		{
			if (!npcDatas.ContainsKey(ps.GetAttribute(i, NPCNAME)))
			{
				string convChunk = ps.GetAttribute(i, 0).Split('0')[0];
				Dictionary<string, List<Dialogue>> convSc = new Dictionary<string, List<Dialogue>>();
				List<Dialogue> dia = new List<Dialogue>();

				Dialogue cur = null;
				switch(int.Parse(ps.GetAttribute(i, CONVTYPE)))
				{
					case 0:
						{
							cur = new Dialogue();
						}
						break;
					case 1:
						{

							cur = new SwapDialogue();
							if(cur is SwapDialogue sw)
							{
								string swapDiaName = ps.GetAttribute(i, SWAPDIA).Trim();
								if (swapDiaName.Length > 0)
								{
									Dictionary<string, List<Dialogue>> target = npcDatas[ps.GetAttribute(i, NPCNAME)];
									Dialogue nxt = null;
									foreach (var allDias in target.Keys)
									{
										if (nxt = target[allDias].Find(x =>
										{
											string[] sp = x.name.Split('_');
											return sp[sp.Length - 1] == swapDiaName;
										}))
										{
											cur.next = nxt;
										}
									}
									if (cur.next == null)
									{
										Debug.LogError($"이름이 {swapDiaName}인 대화는 존재하지 않습니다!");
									}
								}
								else
								{
									cur.next = null;
								}

							}
						}
						break;
					case 2:
						{

							cur = new QuestDialogue();
							if(cur is QuestDialogue qu)
							{
								if(QuestManager.nameQuestPair[ps.GetAttribute(i, QUESTNAME)] != null)
								{
									qu.info = QuestManager.nameQuestPair[ps.GetAttribute(i, QUESTNAME)];
								}
							}
						}
						break;
					case 3:
						{

							cur = new CallbackDialogue(); 
							//쓰면 뒤짐
							//말그대로임 ㅇㅇ
						}
						break;
					case 4:
						{

							cur = new ChoiceDialogue();

							if(cur is ChoiceDialogue ch)
							{

								string nexts;
								while(true)
								{
									if(ch.nexts.Count >= 5)
										break;
									nexts = ps.GetAttribute(i, NEXTDIA + ch.nexts.Count).Trim();
									if(nexts.Length > 0)
										break;
									Dictionary<string, List<Dialogue>> target = npcDatas[ps.GetAttribute(i, NPCNAME)];
									Dialogue nxt = null;
									foreach (var allDias in target.Keys)
									{
										if (nxt = target[allDias].Find(x =>
										{
											string[] sp = x.name.Split('_');
											return sp[sp.Length - 1] == nexts;
										}))
										{
											ch.choiceOptions.Add("???");
											ch.nexts.Add(nxt);
										}
									}
									if (cur.next == null)
									{
										Debug.LogError($"이름이 {nexts}인 대화는 존재하지 않습니다!");
									}
								}


							}
						}
						break;
				}
				cur.rewardItem = ps.GetAttribute(i, REWARDITEM).Trim();
				cur.rewardAmt = int.Parse(ps.GetAttribute(i, REWARDITEM+1));
				cur.text = ps.GetAttribute(i, DIATEXT);
				cur.typeDel = float.Parse(ps.GetAttribute(i, DIATEXT + 1));
				string nextDiaName = ps.GetAttribute(i, NEXTDIA).Trim();
				if (nextDiaName.Length > 0)
				{
					Dictionary<string, List<Dialogue>> target = npcDatas[ps.GetAttribute(i, NPCNAME)];
					Dialogue nxt = null;
					foreach (var allDias in target.Keys)
					{
						if(nxt = target[allDias].Find(x => 
						{
							string[] sp = x.name.Split('_');
							return sp[sp.Length - 1] == nextDiaName;
						}))
						{
							cur.next = nxt;
						}
					}
					if(cur.next == null)
					{
						Debug.LogError($"이름이 {nextDiaName}인 대화는 존재하지 않습니다!");
					}
				}
				else
				{
					cur.next = null;
				}
				cur.name = $"{ps.GetAttribute(i, NPCNAME)}_{ps.GetAttribute(i, 0)}";

				dia.Add(cur);

				convSc.Add(convChunk, dia);


				npcDatas.Add(ps.GetAttribute(i, NPCNAME), convSc);
			}
			else
			{
				string convSection = ps.GetAttribute(i, 0).Split('0')[0];
				if (npcDatas[ps.GetAttribute(i, NPCNAME)].ContainsKey(convSection))
				{

					Dialogue cur = null;
					switch (int.Parse(ps.GetAttribute(i, CONVTYPE)))
					{
						case 0:
							{
								cur = new Dialogue();
							}
							break;
						case 1:
							{

								cur = new SwapDialogue();
								if (cur is SwapDialogue sw)
								{
									string swapDiaName = ps.GetAttribute(i, SWAPDIA).Trim();
									if (swapDiaName.Length > 0)
									{
										Dictionary<string, List<Dialogue>> target = npcDatas[ps.GetAttribute(i, NPCNAME)];
										Dialogue nxt = null;
										foreach (var allDias in target.Keys)
										{
											if (nxt = target[allDias].Find(x =>
											{
												string[] sp = x.name.Split('_');
												return sp[sp.Length - 1] == swapDiaName;
											}))
											{
												cur.next = nxt;
											}
										}
										if (cur.next == null)
										{
											Debug.LogError($"이름이 {swapDiaName}인 대화는 존재하지 않습니다!");
										}
									}
									else
									{
										cur.next = null;
									}

								}
							}
							break;
						case 2:
							{

								cur = new QuestDialogue();
								if (cur is QuestDialogue qu)
								{
									if (QuestManager.nameQuestPair[ps.GetAttribute(i, QUESTNAME)] != null)
									{
										qu.info = QuestManager.nameQuestPair[ps.GetAttribute(i, QUESTNAME)];
									}
								}
							}
							break;
						case 3:
							{

								cur = new CallbackDialogue();
								//쓰면 뒤짐
								//말그대로임 ㅇㅇ
							}
							break;
						case 4:
							{

								cur = new ChoiceDialogue();

								if (cur is ChoiceDialogue ch)
								{

									string nexts;
									while (true)
									{
										if (ch.nexts.Count >= 5)
											break;
										nexts = ps.GetAttribute(i, NEXTDIA + ch.nexts.Count).Trim();
										if (nexts.Length > 0)
											break;
										Dictionary<string, List<Dialogue>> target = npcDatas[ps.GetAttribute(i, NPCNAME)];
										Dialogue nxt = null;
										foreach (var allDias in target.Keys)
										{
											if (nxt = target[allDias].Find(x =>
											{
												string[] sp = x.name.Split('_');
												return sp[sp.Length - 1] == nexts;
											}))
											{
												ch.choiceOptions.Add("???");
												ch.nexts.Add(nxt);
											}
										}
										if (cur.next == null)
										{
											Debug.LogError($"이름이 {nexts}인 대화는 존재하지 않습니다!");
										}
									}


								}
							}
							break;
					}
					cur.rewardItem = ps.GetAttribute(i, REWARDITEM).Trim();
					cur.rewardAmt = int.Parse(ps.GetAttribute(i, REWARDITEM + 1));
					cur.text = ps.GetAttribute(i, DIATEXT);
					cur.typeDel = float.Parse(ps.GetAttribute(i, DIATEXT + 1));
					string nextDiaName = ps.GetAttribute(i, NEXTDIA).Trim();
					if (nextDiaName.Length > 0)
					{
						Dictionary<string, List<Dialogue>> target = npcDatas[ps.GetAttribute(i, NPCNAME)];
						Dialogue nxt = null;
						foreach (var allDias in target.Keys)
						{
							if (nxt = target[allDias].Find(x =>
							{
								string[] sp = x.name.Split('_');
								return sp[sp.Length - 1] == nextDiaName;
							}))
							{
								cur.next = nxt;
							}
						}
						if (cur.next == null)
						{
							Debug.LogError($"이름이 {nextDiaName}인 대화는 존재하지 않습니다!");
						}
					}
					else
					{
						cur.next = null;
					}
					cur.name = $"{ps.GetAttribute(i, NPCNAME)}_{ps.GetAttribute(i, 0)}";

					npcDatas[ps.GetAttribute(i, NPCNAME)][convSection].Add(cur);
				}
			}
		}

		foreach (var item in npcDatas)
		{
			AssetDatabase.CreateFolder(path, item.Key);
			foreach (var data in item.Value)
			{
				AssetDatabase.CreateFolder($"{path}{item.Key}/", data.Key);

				foreach (var dia in data.Value)
				{
					AssetDatabase.CreateAsset(dia, $"{path}{item.Key}/{data.Key}");
				}
			}
		}
	}
}
