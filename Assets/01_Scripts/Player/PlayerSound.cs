using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GroundType
{
	Grass,
	Sand,
	Stone
}
public class PlayerSound : MonoBehaviour
{
    public void FootStepSound(GroundType type, string parameter)
	{
		GameManager.instance.audioPlayer.PlayPoint($"{type}{parameter}", transform.position, 1.0f);
	}
}
