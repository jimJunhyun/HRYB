

public interface ISettings
{
	public bool NotSaved { get; }
	public void Close();
	public void Open();
	public void Revert();

	public void Save();
	public void Load();
}
