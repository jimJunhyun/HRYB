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
	public void OnAnimationStart(Actor self);

	public void OnAnimationMove(Actor self);
	public void OnAnimationEvent(Actor self);

	public void OnAnimationStop(Actor self);
	public void OnAnimationEnd(Actor self);
}

