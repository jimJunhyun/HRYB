using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInter : MonoBehaviour
{
	public float checkRange = 5f;
	List<IInterable> checkeds = new List<IInterable>();

	public void Check()
	{
		List<Collider> founds;
		if ((founds = new List<Collider>(Physics.OverlapSphere(transform.position, checkRange, 1 << 8))).Count > 0)
		{
			checkeds = founds.OrderBy(col => (col.transform.position - transform.position).sqrMagnitude).Select(c => c.GetComponent<IInterable>()).ToList();
			checkeds.RemoveAll(inter => !inter.IsInterable);
			checkeds.RemoveAll(inter => !inter.IsInRange());
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
