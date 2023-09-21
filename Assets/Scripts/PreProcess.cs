using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProcessType
{
    None = -1,

    Trim,
    Grind,
    Roast,
    Pickle,
    Age,
    Steam,
}

public class PreProcess
{
    public ProcessType type;
    public int additionalInfo;
    public string prefix;

    public PreProcess(ProcessType t, int info = -1)
	{
        type = t;
        additionalInfo = info;
		switch (t)
		{
			case ProcessType.None:
				break;
			case ProcessType.Trim:
				prefix = "제";
				break;
			case ProcessType.Grind:
				prefix = "분";
				break;
			case ProcessType.Roast:
				int r = Random.Range(0, 2);
				if(r == 0)
				{
					prefix = "작";
				}
				else
				{
					prefix = "소";
				}
				break;
			case ProcessType.Pickle:
				prefix = $"{(Item.nameHashT[additionalInfo] as YinyangItem).nameAsChar}침";
				break;
			case ProcessType.Age:
				prefix = "숙";
				break;
			case ProcessType.Steam:
				prefix = $"{(Item.nameHashT[additionalInfo] as YinyangItem).nameAsChar}증";
				break;
		}
	}
}
