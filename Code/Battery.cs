using Godot;

namespace ForkliftGame
{
	public partial class Battery : Collectable
	{
		[Export] private double _addedTime = 5.0;
		Timer _timer = null;

		/// <summary>
		/// Add time to level's timer when collected.
		/// Show the visual effect.
		/// </summary>
		/// <param name="forklift"> Only run when forklift collects battery. </param>
		protected override void Collect(Forklift forklift)
		{
			_timer = GetNode<Timer>("../Timer");
			_timer.AddTime(_addedTime);
			LevelManager.Current.ShowBatteryCollectEffect();
		}
	}
}