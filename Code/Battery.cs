using Godot;

namespace ForkliftGame
{
	public partial class Battery : Collectable
	{
		[Export] private double _addedTime = 5.0;
        Timer _timer = null;

		protected override void Collect(Forklift forklift)
		{
			_timer = GetNode<Timer>("../Timer");
			_timer.AddTime(_addedTime);
			LevelManager.Current.ShowBatteryCollectEffect();

		}
	}
}