using System;
using Godot;

namespace ForkliftGame
{
	public partial class SettingsWindow : Control
	{
		[Export] private AudioControl _masterAudioControl = null;
		[Export] private AudioControl _musicAudioControl = null;
		[Export] private AudioControl _effectsAudioControl = null;
		[Export] private TextureButton _exitButton = null;
		[Export] private TextureButton _fiButton = null;
		[Export] private TextureButton _enButton = null;
		[Export] private AudioStreamPlayer _buttonAudio = null;

		private SettingsData _data = null;

		public override void _Ready()
		{
			_data = LoadSettings();
			ApplySettingsData(_data);


			if (_exitButton != null)
			{
				_exitButton.Connect(Button.SignalName.Pressed,
					new Callable(this, nameof(OnExitButtonPressed)));
			}
			if (_fiButton != null)
			{
				_fiButton.Connect(Button.SignalName.Pressed,
					new Callable(this, nameof(OnFiButtonPressed)));
			}
			if (_enButton != null)
			{
				_enButton.Connect(Button.SignalName.Pressed,
					new Callable(this, nameof(OnEnButtonPressed)));
			}
		}
		/// <summary>
		/// Loads settings from save file and opens window.
		/// </summary>
		public virtual void Open()
		{
			_data = LoadSettings();
			ApplySettingsData(_data);
			Show();
		}
		/// <summary>
		/// Closes window.
		/// </summary>
		public virtual void Close()
		{
			Hide();
		}
		/// <summary>
		/// Saves current settings and closes window.
		/// </summary>
		protected virtual void OnExitButtonPressed()
		{
			_buttonAudio.Play();
			SaveSettings();
			Close();
		}
		/// <summary>
		/// Changes games language to Finnish.
		/// </summary>
		private void OnFiButtonPressed()
		{
			_buttonAudio.Play();
			ChangeLanguage("fi");
			_data.LangCode = "fi";
			SaveSettings();

		}
		/// <summary>
		/// Changes games language to English.
		/// </summary>
		private void OnEnButtonPressed()
		{
			_buttonAudio.Play();
			ChangeLanguage("en");
			_data.LangCode = "en";
			SaveSettings();

		}
		/// <summary>
		/// Changes games language.
		/// </summary>
		private void ChangeLanguage(string langCode)
		{
			TranslationServer.SetLocale(langCode);
		}
		/// <summary>
		/// Sets user values to variables.
		/// </summary>
		/// <param name="data">Object which contain variables (settings data)</param>
		private void ApplySettingsData(SettingsData data)
		{
			if (data == null)
			{
				return;
			}

			SetVolume("Master", data.MasterVolume);
			SetVolume("Music", data.MusicVolume);
			SetVolume("SFX", data.SfxVolume);
			ChangeLanguage(data.LangCode);
		}
		/// <summary>
		/// Loads settings from save file.
		/// </summary>
		/// <returns>SettingsData object</returns>
		private SettingsData LoadSettings()
		{
			SettingsData data = null;
			ConfigFile settingsConfig = new ConfigFile();
			if (settingsConfig.Load(Config.SettingsFile) == Error.Ok)
			{
				data = new SettingsData
				{
					LangCode = (string)settingsConfig.GetValue("Localization", "LangCode", "en"),
					MasterVolume = (float)settingsConfig.GetValue("Audio", "MasterVolume", -6.0f),
					MusicVolume = (float)settingsConfig.GetValue("Audio", "MusicVolume", -6.0f),
					SfxVolume = (float)settingsConfig.GetValue("Audio", "SfxVolume", -6.0f)
				};
			}
			else
			{
				data = SettingsData.CreateDefaults();
			}

			Initialize();

			return data;
		}
		/// <summary>
		/// Saves user settings.
		/// </summary>
		/// <returns>true if success</returns>
		public bool SaveSettings()
		{
			if (_data == null)
			{
				return false;
			}

			ConfigFile settingsConfig = new ConfigFile();
			settingsConfig.SetValue("Localization", "LangCode", _data.LangCode);
			settingsConfig.SetValue("Audio", "MasterVolume", _data.MasterVolume);
			settingsConfig.SetValue("Audio", "MusicVolume", _data.MusicVolume);
			settingsConfig.SetValue("Audio", "SfxVolume", _data.SfxVolume);

			if (settingsConfig.Save(Config.SettingsFile) != Error.Ok)
			{
				GD.PrintErr("Failed to save settings.");
				return false;
			}

			return true;
		}
		/// <summary>
		/// Sets volume according to given value in db.
		/// </summary>
		/// <param name="busName">Name of the audio bus</param>
		/// <param name="volumeDB">Value in db</param>
		/// <returns></returns>
		public bool SetVolume(string busName, float volumeDB)
		{
			if (_data == null)
			{
				return false;
			}

			int busIndex = AudioServer.GetBusIndex(busName);
			if (busIndex < 0)
			{
				GD.PrintErr($"Bus '{busName}' not found.");
				return false;
			}

			AudioServer.SetBusVolumeDb(busIndex, volumeDB);

			switch (busName)
			{
				case "Master":
					_data.MasterVolume = volumeDB;
					break;
				case "Music":
					_data.MusicVolume = volumeDB;
					break;
				case "SFX":
					_data.SfxVolume = volumeDB;
					break;
				default:
					GD.PrintErr($"Unknown bus '{busName}'.");
					break;
			}

			return true;
		}
		/// <summary>
		/// Getter for current volume value.
		/// </summary>
		/// <param name="busName">Name of the audio bus</param>
		/// <param name="volumeDB">Value in db</param>
		/// <returns></returns>
		public bool GetVolume(string busName, out float volumeDB)
		{
			int busIndex = AudioServer.GetBusIndex(busName);
			if (busIndex < 0)
			{
				GD.PrintErr($"Bus '{busName}' not found.");
				volumeDB = float.NaN;
				return false;
			}

			volumeDB = AudioServer.GetBusVolumeDb(busIndex);
			return true;
		}
		/// <summary>
		/// Initializes all audio buses.
		/// </summary>
		public void Initialize()
		{
			if (_masterAudioControl != null)
			{
				_masterAudioControl.Initialize();
			}

			if (_musicAudioControl != null)
			{
				_musicAudioControl.Initialize();
			}

			if (_effectsAudioControl != null)
			{
				_effectsAudioControl.Initialize();
			}
		}
	}
}