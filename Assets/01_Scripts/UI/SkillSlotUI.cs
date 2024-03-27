using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlotUI : MonoBehaviour
{
	public SkillSlotInfo slot;
	PlayerCast pCast;

	float curCool;
	float maxCool;

	Image coolDown;
	Image skillIcon;

	private void Awake()
	{
		skillIcon = transform.GetChild(0).GetComponent<Image>();
		coolDown = transform.GetChild(1).GetComponent<Image>();
	}

	private void Start()
	{
		pCast = GameManager.instance.pActor.cast as PlayerCast;
	}


	public void setCooldown()
	{
		curCool = 1 - pCast.nowSkillSlot[(int)slot].CurCooledTime / pCast.nowSkillSlot[(int)slot].skInfo.cooldown;
		skillIcon.sprite = pCast.nowSkillSlot[(int)slot].skInfo.skillIcon;
	}

	private void Update()
	{

		setCooldown();
		coolDown.fillAmount = curCool;
	}

}
