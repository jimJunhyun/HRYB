using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInterable
{
	public static readonly int GlowPowerHash = Shader.PropertyToID("_GlowPower");

	public bool IsInterable { get; set; }
	public bool AltInterable { get;}
	public float InterTime { get; set;}

	public void InteractWith();
	public void AltInterWith();
	public void Inter();
	public void AltInter();
	public void GlowOn();
	public void GlowOff();
}
