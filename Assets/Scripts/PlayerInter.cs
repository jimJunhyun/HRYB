using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInter : MonoBehaviour
{
	public float checkRange = 5f;
	List<IInterable> checkeds = new List<IInterable>();

	Ray r;
	RaycastHit[] hits;

	public void Check()
	{
		r = new Ray(transform.position, transform.forward);
		if ((hits = Physics.SphereCastAll(r, 1.0f,checkRange, (1 << 8))).Length > 0)
		{
			checkeds = hits.OrderByDescending(item => (transform.position - item.point).sqrMagnitude).Select(item => item.collider.GetComponent<IInterable>()).ToList();
			//상호작용 가능 목록 데이터화.
		}
	}

    public void Interact(InputAction.CallbackContext context)
	{
		if(checkeds.Count > 0 && context.performed)
		{
			checkeds[0].InteractWith();
			Check();
		}
	}
}
