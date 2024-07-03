using UnityEngine;

public interface IAnimationEvent
{
	public void OnAnimationStart();

	public void OnAnimationMove();
	public void OnAnimationEvent();

	public void OnAnimationStop();
	public void OnAnimationEnd();
}

public interface IAnimationEventActor
{
	public void OnAnimationStart(Actor self, AnimationEvent evt);

	public void OnAnimationMove(Actor self, AnimationEvent evt);
	public void OnAnimationEvent(Actor self, AnimationEvent evt);

	public void OnAnimationStop(Actor self, AnimationEvent evt);
	public void OnAnimationEnd(Actor self, AnimationEvent evt);

	public void OnAnimationHit(Actor self, AnimationEvent evt);
}


public interface IAnimationEventActorEv
{
	public void OnAnimationStart(AnimationEvent evt);

	public void OnAnimationMove(AnimationEvent evt);
	public void OnAnimationEvent(AnimationEvent evt);

	public void OnAnimationStop(AnimationEvent evt);
	public void OnAnimationEnd(AnimationEvent evt);

	public void OnAnimationHit(AnimationEvent evt);
}

