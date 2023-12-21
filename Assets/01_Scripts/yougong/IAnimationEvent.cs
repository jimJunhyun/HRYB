public interface IAnimationEvent
{
	public void SetAttackRange(int idx);
	public void ResetAttackRange(int idx);

	public void OnAnimationStart();

	public void OnAnimationMove();
	public void OnAnimationEvent();

	public void OnAnimationStop();
	public void OnAnimationEnd();
}
