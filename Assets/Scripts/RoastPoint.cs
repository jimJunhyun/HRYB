using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoastPoint : InterPoint
{
	public YinyangItem holding;

	bool processing = false;
	Roast ongoing = null;

	public override void Inter() //템넣기및빼기
	{
		if(holding == null)
		{

		}
		else
		{
			
		}
	}

	public override void AltInter()//굽기및 취소
	{
		if (!processing)
		{
			ongoing = new Roast(holding);
			ongoing.StartProcess();
			
			processing = true;
		}
		if (processing)
		{
			holding.processes.Add(ongoing);
		}
		
	}
}
