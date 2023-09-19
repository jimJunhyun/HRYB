using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInterable
{
	public bool IsInterable { get; set; }
	public float InterTime { get; set;}
	public void InteractWith();
	public void GlowOn();
}
