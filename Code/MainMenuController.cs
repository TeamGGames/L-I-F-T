using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace ForkliftGame
{
	public partial class MainMenuController : Control
	{
		[Export] private TextureButton _startButton = null;
		[Export] private Button _tutorialButton = null;
		[Export] private Button _settingsButton = null;
		[Export] private Button _creditsButton = null;
		[Export] private TextureButton _musicToggle = null;
		[Export] private SettingsWindow _settingsWindow = null;
		[Export] private CreditsWindow _creditsWindow = null;
		[Export] private SpawnPoint _spawner = null;
		[Export] private double _maxEnergy = 45;
		[Export] private AudioStreamPlayer _buttonAudio = null;
		[Export] private Texture2D _audioONTexture = null;
		[Export] private Texture2D _audioOFFTexture = null;
		private List<string> _levels = new List<string> { Config.Level0, Config.Level1, Config.Level2, Config.Level3 };
		private PackedScene _spawnPointScene = null;
		private string _spawnPointScenePath = "res://GameComponents/SpawnPoint.tscn";
		private SceneTree _mainMenuSceneTree = null;
		private int _nextLevel = 0;
		private Timer _timer = null;


		public override void _Ready()
		{
			_mainMenuSceneTree = GetTree();
			if (_mainMenuSceneTree == null)
			{
				GD.PrintErr("Scene tree not found");
			}
			if (_startButton != null && _settingsButton != null && _tutorialButton != null)
			{
				_startButton.Connect(Button.SignalName.Pressed, new Callable(this, nameof(OnStartPressed)));
				_settingsButton.Connect(Button.SignalName.Pressed, new Callable(this, nameof(OnSettingsPressed)));
				_tutorialButton.Connect(Button.SignalName.Pressed, new Callable(this, nameof(OnTutorialPressed)));
				_creditsButton.Connect(Button.SignalName.Pressed, new Callable(this, nameof(OnCreditsPressed)));
			}

			if (_musicToggle != null)
			{
				_settingsWindow.GetVolume("Master", out float volumeDB);
				if (volumeDB == -80)
				{
					_musicToggle.TextureNormal = _audioOFFTexture;
				}
			}
		}

		/// <summary>
		/// Randomizes and initializes the next level.
		/// </summary>
		public void OnStartPressed()
		{
			if (_buttonAudio != null)
			{
				_buttonAudio.Play();
			}
			if (_spawner == null)
			{
				AddSpawnPointChild();
			}
			// Tutorial level is in _nextLevel index 0
			_nextLevel = GD.RandRange(1, _levels.Count - 1);
			_spawner.fillSpawnerList(_nextLevel);
			LevelMusic.Instance.PlayMusic();
			_mainMenuSceneTree.ChangeSceneToFile(_levels[_nextLevel]);
			InitializeLevel();
		}

		/// <summary>
		/// Starts the tutorial level.
		/// </summary>
		public void OnTutorialPressed()
		{
			_buttonAudio.Play();
			if (_spawner == null)
			{
				AddSpawnPointChild();
			}
			_nextLevel = 0;
			_spawner.fillSpawnerList(_nextLevel);
			LevelMusic.Instance.PlayMusic();
			_mainMenuSceneTree.ChangeSceneToFile(_levels[_nextLevel]);
			InitializeLevel();
		}
		/// <summary>
		/// Mute or umnute the all the sounds in the game
		/// </summary>
		public void OnSoundButtonPressed()
		{
			_settingsWindow.GetVolume("Master", out float volumeDB);
			if (volumeDB != -80)
			{
				_settingsWindow.SetVolume("Master", -80);
			}
			else
			{
				_settingsWindow.SetVolume("Master", -6);
			}
			if (_musicToggle.TextureNormal == _audioONTexture)
			{
				_musicToggle.TextureNormal = _audioOFFTexture;
			}
			else
			{
				_musicToggle.TextureNormal = _audioONTexture;
			}

			_settingsWindow.SaveSettings();
		}

		/// <summary>
		/// Opens the settings window.
		/// </summary>
		private void OnSettingsPressed()
		{
			_buttonAudio.Play();
			if (_settingsWindow != null)
			{
				_settingsWindow.Open();
			}
		}

		/// <summary>
		/// Opens the credits window.
		/// </summary>
		private void OnCreditsPressed()
		{
			_buttonAudio.Play();
			if (_creditsWindow != null)
			{
				_creditsWindow.Open();
			}
		}

		/// <summary>
		/// Initializes the next level.
		/// </summary>
		public void InitializeLevel()
		{
			Godot.Collections.Dictionary saveData = new Godot.Collections.Dictionary();

			// 0 = tutorial level index.
			if (_nextLevel == 0)
			{
				saveData.Add("EnergyLeft", 100);
			}
			else
			{
				saveData.Add("EnergyLeft", _maxEnergy);
			}
			saveData.Add("NextLevel", _nextLevel);
			saveData.Add("Score", 0);

			string savePath = ProjectSettings.GlobalizePath("user://");
			savePath = Path.Combine(savePath, Config.SaveFolder);
			string json = Json.Stringify(saveData);

			if (SaveToFile(savePath, Config.QuickSaveFile, json))
			{
				GD.Print("Game data saved");
			}
			else
			{
				GD.Print("1 Error saving game data");
			}
		}
		/// <summary>
		/// Write save data to external json file.
		/// </summary>
		/// <param name="path"> file path to the save file</param>
		/// <param name="fileName"> name of the save file </param>
		/// <param name="json"> contains all data to be written </param>
		/// <returns>true, if success</returns>
		private bool SaveToFile(string path, string fileName, string json)
		{
			if (!Directory.Exists(path))
			{
				try
				{
					Directory.CreateDirectory(path);
				}
				catch (Exception e)
				{
					GD.PrintErr($"Failed to save game data: {e.Message}");
					return false;
				}
			}

			path = Path.Combine(path, fileName);

			try
			{
				File.WriteAllText(path, json);
			}
			catch (Exception e)
			{
				GD.PrintErr($"Failed to save game data: {e.Message}");
				return false;
			}
			return true;
		}

		/// <summary>
		/// Creates spawnpoint child to scene if needed. Prevents object reference missing errors.
		/// </summary>
		public void AddSpawnPointChild()
		{
			if (_spawnPointScene == null)
			{
				_spawnPointScene = ResourceLoader.Load<PackedScene>(_spawnPointScenePath);
				if (_spawnPointScene == null)
				{
					GD.PrintErr("Spawn scene cannot be found!");
				}
			}
			_spawner = _spawnPointScene.Instantiate<SpawnPoint>();
			AddChild(_spawner);
		}
	}
}