using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class QuestCreater : EditorWindow
{
	string loadName;

	static QuestInfo info;

	System.Text.StringBuilder sb = new System.Text.StringBuilder();
	

	static List<string> allCompleteConditions;
	static List<int> allCompleteConditionsValue;

	static List<string> allCompleteAfters;
	static List<int> allCompleteAftersValue;

	static List<string> allHandleItems;
	static List<int> allHandleItemsValue;

	static List<string> allDefeatEnemies;
	static List<int> allDefeatEnemiesValue;

	static List<string> allRewardTypes;
	static List<int> allRewardTypesValue;

	static List<string> detailParameters;
	static List<int> detailParametersValue;

	static List<string> comparers;
	static List<int> comparersValue;


	static bool conditionOn = false;
	static bool detailOn = false;
	static bool rewardOn = false;
	static List<bool> elementOnOffStat;


	static bool isNewlyCreated = true;

	static QuestInfo originalInfo;


	[MenuItem("퀘스트/퀘스트 생성하기")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(QuestCreater));
	}

	void OnGUI()
	{
		if(allCompleteConditions == null || allCompleteConditionsValue == null)
		{
			List<CompletionAct> lst = new List<CompletionAct>(System.Enum.GetValues(typeof(CompletionAct)).Cast<CompletionAct>());
			if (allCompleteConditions == null)
			{
				allCompleteConditions = new List<string>();
				for (int i = 0; i < lst.Count; i++)
				{
					allCompleteConditions.Add(QuestManager.ToStringKorean(lst[i]));
				}
			}
			if (allCompleteConditionsValue == null)
			{
				allCompleteConditionsValue = new List<int>();
				for (int i = 0; i < lst.Count; i++)
				{
					allCompleteConditionsValue.Add(((int)lst[i]));
				}
			}
		}
		if(allCompleteAfters == null || allCompleteAftersValue == null)
		{
			List<AfterComplete> lst = new List<AfterComplete>(System.Enum.GetValues(typeof(AfterComplete)).Cast<AfterComplete>());
			if (allCompleteAfters == null)
			{
				allCompleteAfters = new List<string>();
				for (int i = 0; i < lst.Count; i++)
				{
					allCompleteAfters.Add(QuestManager.ToStringKorean(lst[i]));
				}
			}
			if (allCompleteAftersValue == null)
			{
				allCompleteAftersValue = new List<int>();
				for (int i = 0; i < lst.Count; i++)
				{
					allCompleteAftersValue.Add(((int)lst[i]));
				}
			}
		}
		if (allHandleItems == null || allHandleItemsValue == null)
		{
			List<ItemHandleMode> lst = new List<ItemHandleMode>(System.Enum.GetValues(typeof(ItemHandleMode)).Cast<ItemHandleMode>());
			if (allHandleItems == null)
			{
				allHandleItems = new List<string>();
				for (int i = 0; i < lst.Count; i++)
				{
					allHandleItems.Add(QuestManager.ToStringKorean(lst[i]));
				}
			}
			if (allHandleItemsValue == null)
			{
				allHandleItemsValue = new List<int>();
				for (int i = 0; i < lst.Count; i++)
				{
					allHandleItemsValue.Add(((int)lst[i]));
				}
			}
		}
		if (allDefeatEnemies == null || allDefeatEnemiesValue == null)
		{
			List<DefeatEnemyMode> lst = new List<DefeatEnemyMode>(System.Enum.GetValues(typeof(DefeatEnemyMode)).Cast<DefeatEnemyMode>());
			if (allDefeatEnemies == null)
			{
				allDefeatEnemies = new List<string>();
				for (int i = 0; i < lst.Count; i++)
				{
					allDefeatEnemies.Add(QuestManager.ToStringKorean(lst[i]));
				}
			}
			if (allDefeatEnemiesValue == null)
			{
				allDefeatEnemiesValue = new List<int>();
				for (int i = 0; i < lst.Count; i++)
				{
					allDefeatEnemiesValue.Add(((int)lst[i]));
				}
			}
		}
		if (allRewardTypes == null || allRewardTypesValue == null)
		{
			List<RewardType> lst = new List<RewardType>(System.Enum.GetValues(typeof(RewardType)).Cast<RewardType>());
			if (allRewardTypes == null)
			{
				allRewardTypes = new List<string>();
				for (int i = 0; i < lst.Count; i++)
				{
					allRewardTypes.Add(QuestManager.ToStringKorean(lst[i]));
				}
			}
			if (allRewardTypesValue == null)
			{
				allRewardTypesValue = new List<int>();
				for (int i = 0; i < lst.Count; i++)
				{
					allRewardTypesValue.Add(((int)lst[i]));
				}
			}
		}
		if(detailParameters == null || detailParametersValue == null)
		{
			List<DetailParameter> lst = new List<DetailParameter>(System.Enum.GetValues(typeof(DetailParameter)).Cast<DetailParameter>());
			if (detailParameters == null)
			{
				detailParameters = new List<string>();
				for (int i = 0; i < lst.Count; i++)
				{
					detailParameters.Add(QuestManager.ToStringKorean(lst[i]));
				}
			}
			if (detailParametersValue == null)
			{
				detailParametersValue = new List<int>();
				for (int i = 0; i < lst.Count; i++)
				{
					detailParametersValue.Add(((int)lst[i]));
				}
			}
		}
		if (comparers == null || comparersValue == null)
		{
			List<Comparer> lst = new List<Comparer>(System.Enum.GetValues(typeof(Comparer)).Cast<Comparer>());
			if (comparers == null)
			{
				comparers = new List<string>();
				for (int i = 0; i < lst.Count; i++)
				{
					comparers.Add(QuestManager.ToStringKorean(lst[i]));
				}
			}
			if (comparersValue == null)
			{
				comparersValue = new List<int>();
				for (int i = 0; i < lst.Count; i++)
				{
					comparersValue.Add(((int)lst[i]));
				}
			}
		}


		GUILayout.Label("퀘스트 정보 설정", EditorStyles.boldLabel);
		GUILayout.Space(10);
		EditorGUILayout.BeginHorizontal();
		loadName = EditorGUILayout.TextField("로드/저장할 파일 이름", loadName);

		if(GUILayout.Button("확인하기"))
		{
			info = Resources.Load<QuestInfo>($"{QuestManager.QUESTINFOPATH}{loadName}");
			Debug.Log("LOAD COMPLETED");
			isNewlyCreated = false;
			if(info != null)
			{
				originalInfo = new QuestInfo(info);
			}
			
			if (info == null)
			{
				info = CreateInstance<QuestInfo>();
				AssetDatabase.CreateAsset(info, $"{QuestManager.ASSETPATH}{QuestManager.QUESTINFOPATH}{loadName}.asset");
				Debug.Log("CREATE COMPLETED");
				isNewlyCreated = true;
				originalInfo = null;
			}
			
			if(info == null)
			{
				//Debug.LogError("CREATE FAILED");
				Close();
			}
		}
		EditorGUILayout.EndHorizontal();

		sb.Clear();

		if (!info)
			return;

		if (elementOnOffStat == null)
		{
			elementOnOffStat = new List<bool>();
			for (int i = 0; i < info.myInfo.Count; i++)
			{
				elementOnOffStat.Add(false);
			}
		}


		GUILayout.Space(10);
		
		info.questName = EditorGUILayout.TextField("퀘스트 이름 (식별용)", info.questName);

		GUILayout.Space(10);

		GUILayout.Label("조건 관리", EditorStyles.boldLabel);
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("새 조건 추가하기"))
		{
			conditionOn = true;
			elementOnOffStat.Add(true);
			info.myInfo.Add(new CompleteAtom());
		}
		if (GUILayout.Button("조건 제거하기"))
		{
			conditionOn = true;
			if(info.myInfo.Count > 0)
			{
				elementOnOffStat.RemoveAt(info.myInfo.Count - 1);
				info.myInfo.RemoveAt(info.myInfo.Count - 1);
			}
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space(5);
		EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1f), Color.black);
		EditorGUILayout.Space(5);

		GUILayout.Label("보상 관리", EditorStyles.boldLabel);
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("새 보상 추가하기"))
		{
			rewardOn = true;
			info.rewardInfo.Add(new RewardAtom());
		}
		if (GUILayout.Button("보상 제거하기"))
		{
			rewardOn = true;
			if (info.rewardInfo.Count > 0)
			{
				info.rewardInfo.RemoveAt(info.rewardInfo.Count - 1);
			}
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space(3);
		EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1f), Color.black);
		EditorGUILayout.Space(3);

		

		if (conditionOn = EditorGUILayout.Foldout(conditionOn, "달성 조건 목록"))
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space(10);
			EditorGUILayout.BeginVertical();
			for (int i = 0; i < info.myInfo.Count; i++)
			{
				if (elementOnOffStat[i] = EditorGUILayout.Foldout(elementOnOffStat[i], $"달성 요소 {i}번째"))
				{
					EditorGUILayout.BeginVertical();

					EditorGUILayout.BeginHorizontal();
					info.myInfo[i].objective = (CompletionAct)EditorGUILayout.IntPopup("조건 : ", ((int)info.myInfo[i].objective), allCompleteConditions.ToArray(), allCompleteConditionsValue.ToArray());
					info.myInfo[i].parameter = EditorGUILayout.TextArea(info.myInfo[i].parameter);
					
					EditorGUILayout.EndHorizontal();

					if (info.myInfo[i].objective == CompletionAct.DefeatTarget)
					{
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.Space(20);
						GUILayout.Label("처치조건 : ");
						info.myInfo[i].defeatParameter = EditorGUILayout.TextArea(info.myInfo[i].defeatParameter);
						info.myInfo[i].defeatMode = (DefeatEnemyMode)EditorGUILayout.IntPopup(((int)info.myInfo[i].defeatMode), allDefeatEnemies.ToArray(), allDefeatEnemiesValue.ToArray());
						
						EditorGUILayout.EndHorizontal();
					}
					else if (info.myInfo[i].objective == CompletionAct.GetItem || info.myInfo[i].objective == CompletionAct.HaveItem)
					{
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.Space(20);
						info.myInfo[i].itemMode = (ItemHandleMode)EditorGUILayout.IntPopup(((int)info.myInfo[i].itemMode), allHandleItems.ToArray(), allHandleItemsValue.ToArray());
						EditorGUILayout.EndHorizontal();
					}

					if(info.myInfo[i].objective == CompletionAct.GetItem || info.myInfo[i].objective == CompletionAct.HaveItem ||
						info.myInfo[i].objective == CompletionAct.LoseItem || info.myInfo[i].objective == CompletionAct.UseItem)
					{
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.Space(25);
						if(GUILayout.Button(info.myInfo[i].itemRequirement ? "세부 조건 있음." : "세부 조건 없음."))
						{
							info.myInfo[i].itemRequirement = !info.myInfo[i].itemRequirement;
						}
						EditorGUILayout.EndHorizontal();
						
						if (info.myInfo[i].itemRequirement)
						{
							EditorGUILayout.BeginHorizontal();
							GUILayout.Space(10);
							if (GUILayout.Button("추가"))
							{
								info.myInfo[i].conds.Add(new DetailCondition());
								detailOn = true;
							}
							if (GUILayout.Button("제거"))
							{
								if(info.myInfo[i].conds.Count > 0)
								{
									info.myInfo[i].conds.RemoveAt(info.myInfo[i].conds.Count - 1);
								}
							}
							

							EditorGUILayout.EndHorizontal();

							if(detailOn = EditorGUILayout.Foldout(detailOn, "세부 조건"))
							{
								EditorGUILayout.BeginVertical();
								GUILayout.Space(10);
								for (int j = 0; j < info.myInfo[i].conds.Count; j++)
								{
									GUILayout.Space(5);
									EditorGUILayout.Space(3);
									EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1f), Color.black);
									EditorGUILayout.Space(3);

									EditorGUILayout.BeginHorizontal();

									info.myInfo[i].conds[j].paramType = (DetailParameter)EditorGUILayout.IntPopup($"{j}번째 조건 : ", ((int)info.myInfo[i].conds[j].paramType), detailParameters.ToArray(), detailParametersValue.ToArray());
									info.myInfo[i].conds[j].comp = (Comparer)EditorGUILayout.IntPopup(((int)info.myInfo[i].conds[j].comp), comparers.ToArray(), comparersValue.ToArray());
									info.myInfo[i].conds[j].value = EditorGUILayout.FloatField(info.myInfo[i].conds[j].value);
									if (GUILayout.Button(info.myInfo[i].conds[j].inverted ? "아닐때" : "일 때"))
									{
										info.myInfo[i].conds[j].inverted = !info.myInfo[i].conds[j].inverted;
									}
									info.myInfo[i].conds[j].after = info.myInfo[i].afterAct = (AfterComplete)EditorGUILayout.IntPopup(((int)info.myInfo[i].conds[j].after), allCompleteAfters.ToArray(), allCompleteAftersValue.ToArray());

									EditorGUILayout.EndHorizontal();
								}
								EditorGUILayout.EndVertical();
							}
							
						}
					}
					
					EditorGUILayout.BeginHorizontal();
					info.myInfo[i].repeatCount = EditorGUILayout.IntField("필요 횟수 : ", info.myInfo[i].repeatCount);
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.Space(30);
					GUILayout.Label("을(를), ");
					if (GUILayout.Button(info.myInfo[i].inverted ? "하지 않았을 때." : "했을 때."))
					{
						info.myInfo[i].inverted = !info.myInfo[i].inverted;
					}
					info.myInfo[i].afterAct = (AfterComplete)EditorGUILayout.IntPopup(((int)info.myInfo[i].afterAct), allCompleteAfters.ToArray(), allCompleteAftersValue.ToArray());
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();
					GUILayout.Label("검사 시작 시점 : ");
					if (GUILayout.Button(info.myInfo[i].everSince ? "게임 시작부터" : "퀘스트 수주이후"))
					{
						info.myInfo[i].everSince = !info.myInfo[i].everSince;
					}

					EditorGUILayout.EndHorizontal();

					EditorGUILayout.EndVertical();
				}
				GUILayout.Space(3);
				EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1f), Color.black);
				GUILayout.Space(3);
			}
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
		}
		
		if(rewardOn = EditorGUILayout.Foldout(rewardOn, "보상 목록"))
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space(5);
			EditorGUILayout.BeginVertical();

			for (int i = 0; i < info.rewardInfo.Count; i++)
			{
				EditorGUILayout.BeginHorizontal();

				info.rewardInfo[i].rewardType = (RewardType)EditorGUILayout.IntPopup($"{i}번째 보상 : ", ((int)info.rewardInfo[i].rewardType), allRewardTypes.ToArray(), allRewardTypesValue.ToArray());
				if(info.rewardInfo[i].rewardType  != RewardType.Exp && info.rewardInfo[i].rewardType != RewardType.HealBlack && info.rewardInfo[i].rewardType != RewardType.HealWhite)
				{
					info.rewardInfo[i].parameter = EditorGUILayout.TextArea(info.rewardInfo[i].parameter);
				}
				if(info.rewardInfo[i].rewardType == RewardType.Item || info.rewardInfo[i].rewardType == RewardType.Exp || info.rewardInfo[i].rewardType == RewardType.HealBlack || info.rewardInfo[i].rewardType == RewardType.HealWhite)
				{
					info.rewardInfo[i].amount = EditorGUILayout.IntField("갯수 : ", info.rewardInfo[i].amount);
				}
				
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(3);
				EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1f), Color.black);
				GUILayout.Space(3);
			}

			EditorGUILayout.EndVertical();

			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("클리어 가능 횟수 : ", EditorStyles.boldLabel);
		info.completableCount = EditorGUILayout.IntField(info.completableCount);
		EditorGUILayout.EndHorizontal();


		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("퀘스트 설명 : ");
		info.descriptions = EditorGUILayout.TextArea(info.descriptions);
		EditorGUILayout.EndHorizontal();


		if (info.questName == "" || info.questName == null)
		{
			sb.AppendLine($"퀘스트 이름이 없습니다.");
		}
		for (int i = 0; i < info.myInfo.Count; i++)
		{
			if (info.myInfo[i].parameter == "" || info.myInfo[i].parameter == null)
			{
				sb.AppendLine($"{i} 번째 조건에 전달될 인수가 없습니다.");
			}

			if (info.myInfo[i].objective == CompletionAct.DefeatTarget && info.myInfo[i].defeatMode != DefeatEnemyMode.None && (info.myInfo[i].defeatParameter == "" || info.myInfo[i].defeatParameter == null))
			{
				sb.AppendLine($"{i} 번째 적 처치 특수 조건에 전달될 인수가 없습니다.");
			}

			if (info.myInfo[i].objective == CompletionAct.None)
			{
				sb.AppendLine($"{i} 번째 조건이 유효하지 않습니다.");
			}
		}

		for (int i = 0; i < info.rewardInfo.Count; i++)
		{
			if ((info.rewardInfo[i].parameter == "" || info.rewardInfo[i].parameter == null))
			{
				sb.AppendLine($"{i} 번째 보상에 전달될 인수가 없습니다.");
			}
		}
		

		GUIStyle warningStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 25, fontStyle = FontStyle.Bold };
		warningStyle.normal.textColor = Color.red;
		warningStyle.hover.textColor = Color.red;

		GUILayout.Label(sb.ToString(), warningStyle);

		if(sb.Length > 0)
			return;

		if (isNewlyCreated)
		{
			GUILayout.Space(100);
			EditorGUILayout.BeginHorizontal("저장");
			if (GUILayout.Button("저장하기..."))
			{
				EditorUtility.SetDirty(info);
				originalInfo = new QuestInfo(info);
				isNewlyCreated = false;
			}
			if (GUILayout.Button("저장하고 작업 종료..."))
			{
				EditorUtility.SetDirty(info);
				Close();
			}
			EditorGUILayout.EndHorizontal();
		}
		else
		{
			GUILayout.Space(100);
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("수정사항 저장하기..."))
			{
				EditorUtility.SetDirty(info);
				originalInfo = new QuestInfo(info);
				Debug.Log(info.descriptions);
			}
			if (GUILayout.Button("수정사항 취소하기..."))
			{
				if(originalInfo != null)
				{
					QuestInfo inf = new QuestInfo(info);

					info.questName = originalInfo.questName;
					info.myInfo = new List<CompleteAtom>(originalInfo.myInfo);
					info.rewardInfo = new List<RewardAtom>(originalInfo.rewardInfo);
					info.completableCount = originalInfo.completableCount;

					originalInfo = new QuestInfo(inf);
					Debug.Log("성공적으로 취소됨.");
				}
			}
			if (GUILayout.Button("수정사항 저장하고 작업 종료..."))
			{
				EditorUtility.SetDirty(info);
				originalInfo = null;
				Close();
			}
			EditorGUILayout.EndHorizontal();
		}
		GUILayout.Space(45);
		GUIStyle dangerous = new GUIStyle(GUI.skin.button) { fontStyle = FontStyle.Bold};
		dangerous.normal.textColor = Color.red;
		dangerous.hover.textColor = Color.red;
		dangerous.focused.textColor = Color.red;
		if (GUILayout.Button("정보 삭제하기", dangerous))
		{
			AssetDatabase.DeleteAsset($"{QuestManager.ASSETPATH}{QuestManager.QUESTINFOPATH}{info.name}.asset");
			info = null;
		}
	}
}