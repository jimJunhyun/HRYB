using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AttackModule))]
[RequireComponent(typeof(MoveModule))]
[RequireComponent(typeof(LifeModule))]
[RequireComponent(typeof(SightModule))]
public class Actor : MonoBehaviour
{
	public AttackModule atk;
	public MoveModule move;
	public LifeModule life;
	public SightModule sight;

	private void Awake()
	{
		atk = GetComponent<AttackModule>();
		move = GetComponent<MoveModule>();
		life = GetComponent<LifeModule>();
		sight = GetComponent<SightModule>();
	}
}
