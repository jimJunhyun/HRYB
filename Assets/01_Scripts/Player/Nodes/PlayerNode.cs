using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public enum BodyPart
{
	Head,
	Body,
	LArm,
	RArm,
	LLeg,
	RLeg,


	Max
}

public class PlayerNode : ScriptableObject
{
	//public int circleIndex;
	public int orderIndex;
	public BodyPart part;

	public bool completed;

	public StatUpgradeType nodeType;
	public string amt;
	public bool percentage;

	public bool learnable;

	public int needPoint;

	public UnityEvent onLearn;

	public List<PlayerNode> requirements;

	public PlayerNode()
	{
		//circleIndex = 0;
		orderIndex = 0;
		completed =false;
		amt = "";
		requirements = new List<PlayerNode>();
		learnable = true;
		needPoint = 0;
	}

	public bool ExamineLearnable()
	{
		bool res = true;

		for (int i = 0; i < requirements.Count; i++)
		{
			res &= requirements[i].completed;
		}
		return learnable && res && GameManager.instance.pinven.currentExp < needPoint;
	}

	public bool LearnNode()
	{
		if(!learnable)
			return false;
		if (GameManager.instance.pinven.currentExp < needPoint)
			return false;

		bool res = true;

		for (int i = 0; i < requirements.Count; i++)
		{
			res &= requirements[i].completed;
		}
		

		if (res)
		{
			 switch (nodeType)
			 {
			 	case StatUpgradeType.Callback:
			 		onLearn?.Invoke();
			 		break;
				case StatUpgradeType.LearnSkill:
					//????????????????
					SkillRoot sk = GameManager.skillLoader.GetHumanSkill(amt);
					if(sk == null)
					{
						sk = GameManager.skillLoader.GetYohoSkill(amt);
					}
					if(sk == null)
						break;
					//스킬을 꽂느냐 배우게 하느냐
					
					break;
			 	default:
					if (percentage)
					{
			 			GameManager.instance.pActor.MultStat(float.Parse(amt), nodeType);
					}
					else
					{
			 			GameManager.instance.pActor.AddStat(float.Parse(amt), nodeType);
					}
			 		break;
			 }
			GameManager.instance.pinven.AddExp(-needPoint);
		}

		return res;
		
	}

	
}
