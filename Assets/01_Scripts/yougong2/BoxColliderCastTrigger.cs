using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoxColliderCastTrigger : BoxColliderCast
{
	[Header("Event")]
	public UnityEvent _OnTriggerEvent;

	private void Start()
	{
		Now(transform, (Actor) => { _OnTriggerEvent?.Invoke(); });
	}
}
