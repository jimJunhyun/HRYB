using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInter : SightModule
{
	public List<IInterable> checkeds = null;

	Ray r;
	RaycastHit[] hits;

	[HideInInspector]
	public int curSel = 0;

	public IInterable curFocused 
	{
		get
		{
			if(checkeds == null)
				return null;
			else
				return checkeds[curSel];
		}
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
		if(checkeds != null && checkeds.Count > 0 && context.performed)
		{
			GameManager.instance.pCast.Cast("interact");
		}
	}

	public void AltInteract(InputAction.CallbackContext context)
	{
		if (checkeds != null && checkeds.Count > 0 && context.performed)
		{
			GameManager.instance.pCast.Cast("altInteract");
		}
	}
}
