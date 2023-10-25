using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackModule : MonoBehaviour
{
	public YinyangWuXing damage;
	public float effSpeed;
	public float effSpeedMod = 1f;
	public float EffSpeed { get => effSpeed * effSpeedMod;}
	public bool isDirect;

	public float prepMod = 1f;

	public float atkGap;

	public virtual void Attack()
	{

	}
}
