using Godot;

namespace ForkliftGame
{
	public partial class Battery : Collectable
	{
		[Export] private double _addedTime = 5.0;
        [Export] Timer _timer = null;

		protected override void Collect(Forklift forklift)
		{
			_timer.AddTime(_addedTime);
            GD.Print("time added");
		}
	}
}