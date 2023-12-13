using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InterType
{
	Insert,
	PickUp,

}

public enum AltInterType
{
	None,
	Process,
	ProcessEnd,

}

public interface IInterable
{
	public static readonly int GlowPowerHash = Shader.PropertyToID("_GlowPower");
	public static readonly int GlowOnHash = Shader.PropertyToID("_IsGlowOn");

	public bool IsInterable { get; set; }
	public bool AltInterable { get;}
	public float InterTime { get; set;}
	public InterType interType { get; set;}
	public AltInterType altInterType { get; set;}

	public void InteractWith();
	public void AltInterWith();
	public void Inter();
	public void AltInter();
	public void GlowOn();
	public void GlowOff();
}
