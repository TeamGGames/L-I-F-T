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
			_volumeSlider.Connect(Slider.SignalName.ValueChanged,
				new Callable(this, nameof(OnVolumeSliderValueChanged)));
		}

		/// <summary>
		/// Set the volume to the default value if  volume value doesn't exist.
		/// Otherwise, set the slider value to the new _originalVolume value.
		/// </summary>
		public void Initialize()
		{
			if (!_settingsWindow.GetVolume(_busName, out _originalVolume))
			{
				GD.PrintErr("Failed to initialize sound volume.");
			}

			_volumeSlider.Value = Mathf.DbToLinear(_originalVolume);
		}

		/// <summary>
		/// First fetch volumeslider's value as a linear value, then change it to decibels
		/// and finally set the new value as decibels.
		/// </summary>
		/// <param name="value"> not used, the Godot method requires it to be in place.</param>
		private void OnVolumeSliderValueChanged(float value)
		{
			float linearVolume = (float)_volumeSlider.Value;
			float decibelVolume = Mathf.LinearToDb(linearVolume);
			_settingsWindow.SetVolume(_busName, decibelVolume);
		}

	}
}