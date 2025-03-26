using System;
using Godot;

namespace ForkliftGame
{
	public partial class SettingsWindow : Control
	{
		[Export] private TextureButton _exitButton = null;
		public override void _Ready()
		{
			if (_exitButton != null)
			{
				_exitButton.Connect(Button.SignalName.Pressed,
					new Callable(this, nameof(OnExitButtonPressed)));
			}
		}

		public virtual void Open()
		{
			Show();
		}

		public virtual void Close()
		{
			Hide();
		}

		protected virtual void OnExitButtonPressed()
		{
			// Handle Cancel button press logic here
			Close();
		}
	}
}