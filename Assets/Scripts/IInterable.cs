using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInterable
{
	public bool IsInterable { get;set; }
	public void InteractWith();
	public bool IsInRange();
}
