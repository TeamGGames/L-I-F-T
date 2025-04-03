using Godot;
using System;

namespace ForkliftGame
{
	public partial class AudioControl : Control
	{
		[Export] private Slider _volumeSlider = null;
		[Export] private string _busName = null;
		private float _originalVolume = 0.0f;
        [Export] private SettingsWindow _settingsWindow = null;

		public override void _Ready()
		{
			base._Ready();

			_volumeSlider.Connect(Slider.SignalName.ValueChanged,
				new Callable(this, nameof(OnVolumeSliderValueChanged)));
		}

		public void Initialize()
		{
			if (!_settingsWindow.GetVolume(_busName, out _originalVolume))
			{
				GD.PrintErr("Äänenvoimakkuuden alustus epäonnistui.");
			}

			_volumeSlider.Value = Mathf.DbToLinear(_originalVolume);
		}

		public void RestoreValues()
		{
			float linearVolume = Mathf.DbToLinear(_originalVolume);
			_volumeSlider.Value = linearVolume;
		}

		private void OnVolumeSliderValueChanged(float value)
		{
			UpdateVolume();
		}

		private void UpdateVolume()
		{
			float linearVolume = (float)_volumeSlider.Value;
			float decibelVolume = Mathf.LinearToDb(linearVolume);
			_settingsWindow.SetVolume(_busName, decibelVolume);
		}
	}
}