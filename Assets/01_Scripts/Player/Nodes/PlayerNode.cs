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

	private void OnEnable()
	{
		completed = false;
	}

	public bool ExamineLearnable()
	{
		bool res = true;

		for (int i = 0; i < requirements.Count; i++)
		{
			res &= requirements[i].completed;
		}
		return !completed && learnable && res && GameManager.instance.pinven.currentExp >= needPoint;
	}

	public void ImmediateLearn()
	{
		switch (nodeType)
		{
			case StatUpgradeType.Callback:
				onLearn?.Invoke();
				break;
			case StatUpgradeType.LearnSkill:
				//$로구분하자.
				string[] skills = amt.Split('$');

				bool foxSkill = false;
				SkillRoot sk = GameManager.instance.saver.skillLoader.GetHumanSkill(skills[2]);
				if (sk == null)
				{
					sk = GameManager.instance.saver.skillLoader.GetYohoSkill(skills[2]);
					foxSkill = true;
				}

				if (sk == null)
					break;
				//스킬을 꽂는다.
				switch (skills[1])
				{
					case "Q":
						if (foxSkill)
						{
							(GameManager.instance.pActor.cast as PlayerCast).ConnectSkillDataTo(sk, SkillSlotInfo.Q, PlayerForm.Yoho);
						}
						else
						{
							(GameManager.instance.pActor.cast as PlayerCast).ConnectSkillDataTo(sk, SkillSlotInfo.Q, PlayerForm.Magic);
						}
						break;
					case "E":
						if (foxSkill)
						{
							(GameManager.instance.pActor.cast as PlayerCast).ConnectSkillDataTo(sk, SkillSlotInfo.E, PlayerForm.Yoho);
						}
						else
						{
							(GameManager.instance.pActor.cast as PlayerCast).ConnectSkillDataTo(sk, SkillSlotInfo.E, PlayerForm.Magic);
						}
						break;
					case "R":
						if (foxSkill)
						{
							(GameManager.instance.pActor.cast as PlayerCast).ConnectSkillDataTo(sk, SkillSlotInfo.One, PlayerForm.Yoho);
						}
						else
						{
							(GameManager.instance.pActor.cast as PlayerCast).ConnectSkillDataTo(sk, SkillSlotInfo.One, PlayerForm.Magic);
						}
						break;
					case "RMB":
						if (foxSkill)
						{
							(GameManager.instance.pActor.cast as PlayerCast).ConnectSkillDataTo(sk, SkillSlotInfo.RClick, PlayerForm.Yoho);
						}
						else
						{
							(GameManager.instance.pActor.cast as PlayerCast).ConnectSkillDataTo(sk, SkillSlotInfo.RClick, PlayerForm.Magic);
						}
						break;

					default:
						break;
				}
				break;
			default:
				if (percentage)
				{
					GameManager.instance.pActor.MultStat(float.Parse(amt) / 100f, nodeType);
					Debug.Log("스탯 : " + nodeType + " * " + float.Parse(amt) + " % ");
				}
				else
				{
					GameManager.instance.pActor.AddStat(float.Parse(amt), nodeType);
					Debug.Log("스탯 : " + nodeType + " + " + float.Parse(amt));
				}
				break;
		}
		completed = true;
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
					//$로구분하자.
					string[] skills = amt.Split('$');

					bool foxSkill = false;
					SkillRoot sk = GameManager.instance.saver.skillLoader.GetHumanSkill(skills[2]);
					if(sk == null)
					{
						sk = GameManager.instance.saver.skillLoader.GetYohoSkill(skills[2]);
						foxSkill = true;
					}

					if(sk == null)
						break;
					//스킬을 꽂는다.
					switch (skills[1])
					{
						case "Q":
							if(foxSkill)
							{
								(GameManager.instance.pActor.cast as PlayerCast).ConnectSkillDataTo(sk, SkillSlotInfo.Q, PlayerForm.Yoho);
							}
							else
							{
								(GameManager.instance.pActor.cast as PlayerCast).ConnectSkillDataTo(sk, SkillSlotInfo.Q, PlayerForm.Magic);
							}
							break;
						case "E":
							if (foxSkill)
							{
								(GameManager.instance.pActor.cast as PlayerCast).ConnectSkillDataTo(sk, SkillSlotInfo.E, PlayerForm.Yoho);
							}
							else
							{
								(GameManager.instance.pActor.cast as PlayerCast).ConnectSkillDataTo(sk, SkillSlotInfo.E, PlayerForm.Magic);
							}
							break;
						case "R":
							if (foxSkill)
							{
								(GameManager.instance.pActor.cast as PlayerCast).ConnectSkillDataTo(sk, SkillSlotInfo.One, PlayerForm.Yoho);
							}
							else
							{
								(GameManager.instance.pActor.cast as PlayerCast).ConnectSkillDataTo(sk, SkillSlotInfo.One, PlayerForm.Magic);
							}
							break;
						case "RMB":
							if (foxSkill)
							{
								(GameManager.instance.pActor.cast as PlayerCast).ConnectSkillDataTo(sk, SkillSlotInfo.RClick, PlayerForm.Yoho);
							}
							else
							{
								(GameManager.instance.pActor.cast as PlayerCast).ConnectSkillDataTo(sk, SkillSlotInfo.RClick, PlayerForm.Magic);
							}
							break;

						default:
							break;
					}
					break;
			 	default:
					if (percentage)
					{
			 			GameManager.instance.pActor.MultStat(float.Parse(amt) / 100f, nodeType);
						Debug.Log("스탯 : " + nodeType + " * " + float.Parse(amt) + " % ");
					}
					else
					{
			 			GameManager.instance.pActor.AddStat(float.Parse(amt), nodeType);
						Debug.Log("스탯 : " + nodeType + " + " + float.Parse(amt));
					}
			 		break;
			 }
			completed = true;
			GameManager.instance.pinven.AddExp(-needPoint);
		}

		return res;
		
	}

	
}
