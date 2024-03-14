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
}

