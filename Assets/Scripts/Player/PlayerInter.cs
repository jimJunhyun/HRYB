using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInter : SightModule
{
	const float ALTINTERTIME = 0.5f;

	public List<IInterable> checkeds = null;

	public float holdTime = 0.5f;

	Ray r;
	RaycastHit[] hits;

	[HideInInspector]
	public int curSel = 0;

	bool holding = false;

	float pressStart = 0;
	float pressStop = 0;

	public IInterable curFocused 
	{
		get
		{
			if(checkeds == null || checkeds.Count <= curSel)
				return null;
			else
				return checkeds[curSel];
		}
	}

	private void Update()
	{
		if(holding)
			GameManager.instance.uiManager.preInterUI.SetGaugeValue(Mathf.Clamp01((Time.time - pressStart) / 0.5f));
	}

	public void Check()
	{
		r = new Ray(transform.position, transform.forward);
		if (checkeds != null)
		{
			for (int i = 0; i < checkeds.Count; i++)
			{
				checkeds[i].GlowOff();
			}
		}
		if ((hits = Physics.SphereCastAll(r, 1.0f, sightRange, (1 << 8))).Length > 0)
		{
			checkeds = hits.OrderByDescending(item => (transform.position - item.point).sqrMagnitude).Select(item => item.collider.GetComponent<IInterable>()).ToList();
			curSel %= checkeds.Count;
			checkeds[curSel].GlowOn();
			if (checkeds[curSel].IsInterable)
			{
				GameManager.instance.uiManager.preInterUI.On();
			}
			if (checkeds[curSel].AltInterable)
			{

			}
		}
		else
		{
			if (checkeds != null)
			{
				for (int i = 0; i < checkeds.Count; i++)
				{
					checkeds[i].GlowOff();
				}
				checkeds.Clear();
			}
			curSel = 0;
			GameManager.instance.uiManager.preInterUI.Off();
		}
	}

	public void ScrollThrough(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			if (checkeds != null && checkeds.Count > 0)
			{
				checkeds[curSel].GlowOff();
				Vector2 scr = context.ReadValue<Vector2>();
				if (scr.y > 0)
				{
					curSel += 1;
					curSel %= checkeds.Count;
				}
				else if (scr.y < 0)
				{
					curSel += checkeds.Count - 1;
					curSel %= checkeds.Count;
				}
				checkeds[curSel].GlowOn();
			}
		}
		
		
	}

    public void Interact(InputAction.CallbackContext context)
	{
		if(GameManager.instance.pinven.stat == HandStat.Item)
		{
			if (context.performed)
			{
				pressStart = Time.time;
				holding = true;
			}

			if (checkeds != null && checkeds.Count > 0 && context.canceled)
			{
				holding = false;
				GameManager.instance.uiManager.preInterUI.SetGaugeValue(0);
				pressStop = Time.time;
				if ((pressStop - pressStart) < 0.5f || (!curFocused.AltInterable))
				{
					(GetActor().anim as PlayerAnim).SetInteractTrigger();
					GetActor().cast.Cast("interact");
				}
				else
				{
					(GetActor().anim as PlayerAnim).SetInteractTrigger();
					GetActor().cast.Cast("altInteract");
				}
			}
		}
		
	}

}
