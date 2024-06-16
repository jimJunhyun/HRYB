using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
{
	public AnimationClipOverrides(int capacity) : base(capacity) { }

	public AnimationClip this[string name]
	{
		get { return this.Find(x => x.Key.name.Equals(name)).Value; }
		set
		{
			int index = this.FindIndex(x => x.Key.name.Equals(name));
			if (index != -1)
				this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
		}
	}
}

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

	protected AnimatorOverrideController animatorOverrideController;

	protected AnimationClipOverrides clipOverrides;

	public virtual void Awake()
	{
		anim = GetComponent<Animator>();
		if (anim.runtimeAnimatorController != null)
		{
			animatorOverrideController = new AnimatorOverrideController(anim.runtimeAnimatorController);
			Animators.runtimeAnimatorController = animatorOverrideController;

			clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
			animatorOverrideController.GetOverrides(clipOverrides);
		}

	}


	public virtual void SetAttackTrigger()
	{
		anim.SetTrigger(attackHash);
	}

	public virtual void SetHitTrigger()
	{
		anim.SetTrigger(hitHash);
		if(hitClipName != "")
		{
			GameManager.instance.audioPlayer.PlayPoint(hitClipName, transform.position);
		}
	}

	public virtual void SetDieTrigger()
	{
		anim.SetTrigger(dieHash);
	}

	public virtual void SetMoveState(int val = 0)
	{
		anim.SetFloat(moveHash, val);
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
		anim.SetBool(dieHash, false);
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

	public void SetIntigerModify(string a, int b)
	{
		anim.SetInteger(Animator.StringToHash(a), b);
	}

	public virtual void SetAnimationOverrides(List<string> from, List<AnimationClip> to)
	{

		for (int i = 0; i < from.Count; i++)
		{
			if (i < to.Count)
			{
				clipOverrides[from[i]] = to[i];
			}
		}
		animatorOverrideController.ApplyOverrides(clipOverrides);

	}

	public void SetChangeAnimation(string id, AnimationClip _clip)
	{
		Debug.LogError(clipOverrides);
		clipOverrides[id] = _clip;
		animatorOverrideController.ApplyOverrides(clipOverrides);
	}

	public void StartExampled()
	{ 
		if(self.AI != null)
			self.AI.StartExamine();
	}
}
