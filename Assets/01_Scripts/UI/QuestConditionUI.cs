using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestConditionUI : MonoBehaviour
{
    TextMeshProUGUI condText;
	//TextMeshProUGUI compCount;

	CompleteAtom cur;

	public void ShowCond(CompleteAtom comp)
	{
		if(condText == null)
		{
			condText = transform.Find("CompletionAtomDesc").GetComponent<TextMeshProUGUI>();
		}
		//if(compCount == null)
		//{
		//	compCount = transform.Find("CompletionAtomCount").GetComponent<TextMeshProUGUI>();
		//}

		cur = comp;

		System.Text.StringBuilder sb;
		bool usingGlobal = GameManager.GetGlobalSB(out sb);

		sb.Append("<#00dd00>");
		if(comp.objective == CompletionAct.EqualLevel || comp.objective == CompletionAct.AboveLevel)
		{
			sb.Append("레벨 ");
		}
		sb.Append(comp.parameter);
		sb.Append("</color>을(를) ");
		if(comp.objective == CompletionAct.DefeatTarget)
		{
			sb.Append("<#0000dd>");
			sb.Append(comp.defeatParameter);
			sb.Append("</color>");
			switch (comp.defeatMode)
			{
				case DefeatEnemyMode.Form:
					sb.Append(" 형태로");
					break;
				case DefeatEnemyMode.Skill:
					sb.Append(" 스킬로");
					break;
				case DefeatEnemyMode.Element:
					sb.Append(" 속성으로");
					break;
				case DefeatEnemyMode.OverDamage:
					sb.Append(" 이상 피해로");
					break;
				case DefeatEnemyMode.DamageType:
					sb.Append(" 피해로");
					break;
				case DefeatEnemyMode.StatusEffect:
					sb.Append(" 상태를 부여해");
					break;
				case DefeatEnemyMode.SelfStatusEffect:
					sb.Append(" 상태를 가지고");
					break;
				default:
					break;
			}
		}
		sb.Append("<#cc00cc>");
		switch (comp.objective)
		{
			case CompletionAct.GetItem:
				sb.Append("획득하");
				break;
			case CompletionAct.HaveItem:
				sb.Append("소지하");
				break;
			case CompletionAct.LoseItem:
				sb.Append("잃");
				break;
			case CompletionAct.UseItem:
				sb.Append("사용하");
				break;
			case CompletionAct.DefeatTarget:
				sb.Append("처치하");
				break;
			case CompletionAct.CountSecond:
				sb.Append("대기하");
				break;
			case CompletionAct.MoveTo:
				sb.Append("이동하");
				break;
			case CompletionAct.InteractWith:
				sb.Append("대화하");
				break;
			case CompletionAct.EqualLevel:
				sb.Append("달성하");
				break;
			case CompletionAct.AboveLevel:
				sb.Append("초과하");
				break;
			case CompletionAct.GoNear:
				sb.Append("다가가");
				break;
			case CompletionAct.RemainNear:
				sb.Append("근처에 머무르");
				break;
			case CompletionAct.ClearQuest:
				sb.Append("해결하");
				break;
			case CompletionAct.LearnSkill:
				sb.Append("배우");
				break;
		}

		if (comp.inverted)
		{
			sb.Append("지</color> <#dd0000>않기</color>");
		}
		else
		{
			sb.Append("기</color>");
		}

		condText.text = sb.ToString();
		sb.Clear();

		//if (comp.isCompleted)
		//{
		//	sb.Append("<#00dd00>");
		//}
		//else
		//{
		//	sb.Append("<#dd0000>");
		//}

		//sb.Append(comp.curRepeatCount);
		//sb.Append("</color>");
		//sb.Append(" / ");
		//sb.Append(comp.repeatCount);

		//compCount.text = sb.ToString();
		//sb.Clear();

		GameManager.ReturnGlobalSB(usingGlobal);
	}

	public void RefreshCount()
	{
		//System.Text.StringBuilder sb;
		//bool usingGlobal = GameManager.GetGlobalSB(out sb);

		//if (cur.isCompleted)
		//{
		//	sb.Append("<#dd0000>");
		//}
		//else
		//{
		//	sb.Append("<#00dd00>");
		//}

		//sb.Append(cur.curRepeatCount);
		//sb.Append("</color>");
		//sb.Append(" / ");
		//sb.Append(cur.repeatCount);

		//compCount.text = sb.ToString();


		//GameManager.ReturnGlobalSB(usingGlobal);
	}
}
