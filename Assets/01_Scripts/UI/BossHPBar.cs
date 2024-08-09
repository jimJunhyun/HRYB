using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHPBar : MonoBehaviour
{
	public LifeModule boyLf;
	public LifeModule girlLf;

	public Image boyHp;
	public Image girlHp;

	public Image boyWhiteAdequity;
	public Image girlWhiteAdequity;

	private void Update()
	{
		boyHp.fillAmount = boyLf.yy.white.Value / boyLf.yy.white.MaxValue;
		girlHp.fillAmount = girlLf.yy.white.Value / girlLf.yy.white.MaxValue;

		boyWhiteAdequity.fillAmount = boyLf.adequity.white.Value / (boyLf.adequity.white.Value + boyLf.adequity.black.Value);
		girlWhiteAdequity.fillAmount = girlLf.adequity.white.Value / (girlLf.adequity.white.Value + girlLf.adequity.black.Value);
	}

}
