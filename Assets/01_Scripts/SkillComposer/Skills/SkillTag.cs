using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum SkillTags
{
	None = 0,

	AttackEnhance = 1,
	NonAttackEnhance = 2,
	AttackEnhancable = 4,
	NonAttackEnhancable = 8,
	AttackConstant = 16,
	NonAttackConstant = 32,
	Special = 64,



	All = -1,
}

[Serializable]
public struct SkillTag
{
    public int value;
	public void AddTag(SkillTags tag)
	{
		value |= ((int)tag);
	}
	public void AddTags(SkillTag tagInfo)
	{
		value |= tagInfo.value;
	}
	public void RemoveTag(SkillTags tag)
	{
		int t = ((int)tag);
		t &= value;
		value ^= t;
	}
	public void RemoveTags(SkillTag tagInfo)
	{
		int t = tagInfo.value;
		t &= value;
		value ^= t;
	}
	public bool ContainsTag(params SkillTags[] objs)
	{
		bool res = true;
		for (int i = 0; i < objs.Length; i++)
		{
			int digit = (int)Mathf.Log(((int)objs[i]), 2);
			res &= (value >> digit) % 2 == 1;
		}
		return res;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(value);
	}

	public override string ToString()
	{
		StringBuilder sb = new StringBuilder();
		sb.Append(value.ToString());
		sb.Append(" : ");
		int v = value;
		Array arr = Enum.GetValues(typeof(SkillTags));
		int head = 1;
		if(v == 0)
		{
			sb.Append("None");
		}
		else if(v == -1)
		{
			sb.Append("All");
		}
		else
		{
			while (v > 0)
			{
				if (v % 2 == 1)
				{
					sb.Append(arr.GetValue(head).ToString());
					sb.Append(", ");
				}
				v = v >> 1;
				head += 1;
			}
		}
		
		return sb.ToString();
	}

	public override bool Equals(object obj)
	{
		return obj is SkillTag tag &&
			   value == tag.value;
	}

	public static bool ContainsTag(int val, params SkillTags[] objs)
	{
		bool res = true;
		for (int i = 0; i < objs.Length; i++)
		{
			int digit = (int)Mathf.Log(((int)objs[i]), 2);
			res &= (val >> digit) % 2 == 1;
		}
		return res;
	}

	
}
