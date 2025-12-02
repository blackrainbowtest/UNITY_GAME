namespace WorldLogic
{
	/// <summary>
	/// A world object manager that lives at runtime.
	/// Receives ready-made data from the generator and manages it during gameplay.
	/// </summary>
	public interface IWorldManager
	{
		void Initialize();
	}
}