using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class HPBar : MonoBehaviour
{
	

	EnemyLifeModule lf;
	Image hp;
	TextMeshProUGUI nameText;

	Image whiteAdequity;
	Image groge;

	Canvas parent;

	private void Awake()
	{
		hp = transform.Find("HPBar/HP").GetComponent<Image>();
		lf = GetComponentInParent<EnemyLifeModule>();
		nameText = transform.Find("NameBack/NameText").GetComponent<TextMeshProUGUI>();
		whiteAdequity = transform.Find("BlackBack/WhiteBack").GetComponent<Image>();
		groge = transform.Find("GrogyBar/Grogy").GetComponent<Image>();
		parent = GetComponent<Canvas>();

		nameText.text = lf.GetActor().actorName;
	}

	private void Update()
	{
		hp.fillAmount = lf.yy.white.Value / lf.yy.white.MaxValue;
		whiteAdequity.fillAmount = lf.adequity.white.Value / (lf.adequity.white.Value + lf.adequity.black.Value);
		groge.fillAmount = lf.GetGrogeValue  / lf._grogeInitValue;

		parent.enabled = true;
	}
}
