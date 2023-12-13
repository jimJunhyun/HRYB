public interface IAnimationEvent
{
	public void SetAttackRange(int idx);
	public void ResetAttackRange(int idx);
	public void OnAnimationEvent();

	public void OnAnimationStop();
	public void OnAnimationEnd();
}
