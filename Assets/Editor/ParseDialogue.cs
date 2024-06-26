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
	const int NEXTDIA = 8;

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

							}
						}
						break;
					case 2:
						{

							cur = new QuestDialogue();
							if(cur is QuestDialogue qu)
							{

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

							}
						}
						break;
				}
				cur.text = ps.GetAttribute(i, DIATEXT);
				cur.typeDel = float.Parse(ps.GetAttribute(i, DIATEXT + 1));
				if (ps.GetAttribute(i, NEXTDIA).Trim().Length > 0)
				{

				}
				else
				{
					cur.next = null;
				}

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
							}
							break;
						case 2:
							{

								cur = new QuestDialogue();
							}
							break;
						case 3:
							{

								cur = new CallbackDialogue();
							}
							break;
						case 4:
							{

								cur = new ChoiceDialogue();
							}
							break;
					}
					cur.text = ps.GetAttribute(i, DIATEXT);
					cur.typeDel = float.Parse(ps.GetAttribute(i, DIATEXT + 1));
					if(ps.GetAttribute(i, NEXTDIA).Trim().Length > 0)
					{

					}
					else
					{
						cur.next = null;
					}

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
