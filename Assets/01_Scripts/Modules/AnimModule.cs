using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimModule : Module
{
	public string hitClipName;

	protected readonly int moveHash = Animator.StringToHash("Move");
	protected readonly int idleHash = Animator.StringToHash("Idle");
	
	protected readonly int attackHash = Animator.StringToHash("Attack");
	protected readonly int moveXHash = Animator.StringToHash("MoveX");
	protected readonly int moveYHash = Animator.StringToHash("MoveY");
	protected readonly int hitHash = Animator.StringToHash("Hit");
	protected readonly int dieHash = Animator.StringToHash("Die");
	protected readonly int respawnHash = Animator.StringToHash("Respawn");




	protected Animator anim;
	public Animator Animators => anim;
	public virtual void Awake()
	{
		anim = GetComponent<Animator>();
	}


	public virtual void SetAttackTrigger()
	{
		anim.SetTrigger(attackHash);
	}

	public virtual void SetHitTrigger()
	{
		anim.SetTrigger(hitHash);
		GameManager.instance.audioPlayer.PlayPoint(hitClipName, transform.position);
	}

	public virtual void SetDieTrigger()
	{
		anim.SetTrigger(dieHash);
	}

	public virtual void SetMoveState(int val = 0)
	{
		anim.SetInteger(moveHash, val);
	}
	public virtual void SetMoveState(bool b)
	{
		anim.SetBool(moveHash, b);
	}

	public virtual void SetIdleState(bool val)
	{
		anim.SetBool(idleHash, val);
	}

	public override void ResetStatus()
	{
		base.ResetStatus();

		anim.SetTrigger(respawnHash);
	}

	public void SetTrigger(int hash)
	{
		anim.SetTrigger(hash);
	}

	public void SetBoolModify(string a, bool b)
	{
		anim.SetBool(Animator.StringToHash(a), b);
	}

	public virtual void SetAnimationOverrides(List<string> from, List<AnimationClip> to)
	{
		AnimatorOverrideController ctrl = new AnimatorOverrideController(anim.runtimeAnimatorController);
		List<KeyValuePair<AnimationClip, AnimationClip>> apply = new List<KeyValuePair<AnimationClip, AnimationClip>>();

		//for (int i = 0; i < ctrl.animationClips.Length; i++)
		//{
		//	//Debug.Log($"Examining : {ctrl.animationClips[i].name}");
		//	int idx = from.FindIndex(n => n == ctrl.animationClips[i].name);
		//
		//	Debug.LogWarning(ctrl.animationClips[i].name);
		//
		//	if (idx != -1 && idx < to.Count)
		//	{
		//		//Debug.Log($"New Animation To : {to[idx].GetInstanceID()}");
		//		apply.Add(new KeyValuePair<AnimationClip, AnimationClip>(ctrl.animationClips[i], to[idx]));
		//	}
		//}

		for(int i =0 ; i< from.Count; i++)
		{
			if(i < to.Count)
			{
				ctrl[$"{from[i]}"] = to[i];
			}
		}
		anim.runtimeAnimatorController = ctrl;
		
	}
	
	public void StartExampled()
	{
		self._ai.StartExamine();
	}
}
