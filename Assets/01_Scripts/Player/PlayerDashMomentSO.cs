using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ETC/PlayerDashSO")]
public class PlayerDashMomentSO : ScriptableObject
{
	public AnimationClip Front;
	public AnimationClip Back;
	public AnimationClip Left;
	public AnimationClip Right;
}
