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

	[Export] private Button _startButton = null;
	[Export] private Button _settingsButton = null;
	[Export] private Button _instructionsButton = null;

	[Export] private DialogWindow _settingsWindow = null;
	[Export] private DialogWindow _instructionsWindow = null;
	[Export] private SpawnPoint _spawner = null;

	private List<string> _levels = new List<string> {Config.Level1, Config.Level2};

	private SceneTree _mainMenuSceneTree = null;
	private int _nextLevel = 0;

	private Timer _timer = null;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
			base._Ready();
			_mainMenuSceneTree = GetTree();
			if (_mainMenuSceneTree == null)
			{
				GD.PrintErr("Scene tree not found");
			}

			_startButton.Connect(Button.SignalName.Pressed, new Callable(this, nameof(OnStartPressed)));
			_settingsButton.Connect(Button.SignalName.Pressed, new Callable(this, nameof(OnSettingsPressed)));
			_instructionsButton.Connect(Button.SignalName.Pressed, new Callable(this, nameof(OnInstructionsPressed)));

	}

	private void OnStartPressed()
	{
		GD.Print("New game pressed");
		_nextLevel = GD.RandRange(0 ,_levels.Count - 1);
		_spawner.fillSpawnerList(_nextLevel);
		_mainMenuSceneTree.ChangeSceneToFile(_levels[_nextLevel]);
		InitializeLevel();
	}

	private void OnSettingsPressed()
	{
		if (_settingsWindow != null)
		{
			_settingsWindow.Open();
		}
	}

	private void OnInstructionsPressed()
	{
		if (_instructionsWindow != null)
		{
			_instructionsWindow.Open();
		}
	}



	public void InitializeLevel()
	{
		Godot.Collections.Dictionary saveData = new Godot.Collections.Dictionary();

			saveData.Add("EnergyLeft", 30);
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
	private bool SaveToFile(string path, string fileName, string json)
	{
		if ( !Directory.Exists(path))
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

}
}