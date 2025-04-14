using System;
using Godot;

namespace ForkliftGame
{
	public partial class CreditsWindow : Control
	{
		[Export] private Button _exitButton = null;
        [Export] private AudioStreamPlayer _buttonAudio = null;



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
			_buttonAudio.Play();
			Close();
		}



	}
}