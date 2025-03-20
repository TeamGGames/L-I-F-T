using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;


namespace ForkliftGame
{

/// <summary>
/// Currently the Testlevel.cs handles keeping track of the score
/// </summary>
public partial class LevelManager : Node2D
{
	[Export] private string _forkliftScenePath = "res://GameComponents/Forklift.tscn";
	[Export] private string _boxScenePath = "res://GameComponents/box.tscn";
	[Export] private string _batteryScenePath = "res://GameComponents/BatteryPowerUp.tscn";
	[Export] private string _spawnPointScenePath = "res://GameComponents/SpawnPoint.tscn";
	[Export] private string _timerScenePath = "res://GameComponents/Timer.tscn";

	[Export] private Ui _ui = null;
	[Export] private SpawnPoint _spawner = null;

	private SceneTree _levelSceneTree = null;
	private PackedScene _forkliftScene = null;
	private PackedScene _boxScene = null;
	private PackedScene _batteryScene = null;
	private PackedScene _spawnPointScene = null;
	private PackedScene _timerScene = null;
	private static LevelManager _current = null;

	private Forklift _forklift = null;
	private Box _box = null;
	private Timer _timer = null;
	private SpawnPoint _spawnPoint = null;
	[Export] private LoadingArea _loadingArea = null;
	private Battery _battery = null;
			public static LevelManager Current
			{
				get { return _current; }
			}

	private int _score = 0;

	public int Score {
		get { return _score; }
		set { _score = value; }
	}

	private List<Box> _spawnedBoxes = new List<Box>();
	private List<SpawnPoint> _spawnPoints = new List<SpawnPoint>();
	private List<string> _levels = new List<string> {Config.Level1, Config.Level2};

	public int _nextLevel = 0;
	public bool _isGameOver = false;


    public override void _Ready()
    {
        _current = this;
		StartGame();
    }

	private Forklift CreateForklift()
	{
		if (_forkliftScene == null)
		{
			_forkliftScene = ResourceLoader.Load<PackedScene>(_forkliftScenePath);
			if (_forkliftScene == null)
			{
				GD.PrintErr("Forklift scene cannot be found!");
				return null;
			}
		}
		return _forkliftScene.Instantiate<Forklift>();
	}
	private Timer CreateTimer()
	{
		if (_timerScene == null)
		{
			_timerScene = ResourceLoader.Load<PackedScene>(_timerScenePath);
			if (_timerScene == null)
			{
				GD.PrintErr("Timer scene cannot be found!");
				return null;
			}
		}
		return _timerScene.Instantiate<Timer>();
	}

	#region public methods
    public void PrintScore()
	{
		GD.Print($"Score: {Score}");
	}

	public void StartGame()
	{
		DestroyForklift();
		_forklift = CreateForklift();
		AddChild(_forklift);
		DestroyTimer();
		_timer = CreateTimer();
		AddChild(_timer);
		_timer.Reset(true);

		if(!Load())
		{
			GD.PrintErr("Load error");
		}

		_spawner.fillSpawnerList(_nextLevel);

		_forklift.GlobalPosition = _loadingArea.SpawnPosition;
		Score = 0;


		DestroyBoxes();

		for (int i = 0; i < 3; i++)
		{
			SpawnBox(CreateSpawnPoints());
		}

		SpawnBattery(CreateSpawnPoints());
	}

	public void DestroyForklift()
	{
		if (_forklift != null)
		{
			_forklift.QueueFree();
			_forklift = null;
		}
	}

	public void DestroyTimer()
	{
		if (_timer != null)
		{
			_timer.QueueFree();
			_timer = null;
		}
	}

	public void DestroyBoxes()
	{
		if (_box != null)
		{
			_box.QueueFree();

			_box = null;
		}

		for (int i = 0; i < _spawnedBoxes.Count; i++)
		{
			_spawnedBoxes[i].QueueFree();
		}
		_spawnedBoxes.Clear();
	}
	public void SpawnBox(Vector2 spawnPosition)
	{

		if (_boxScene == null)
		{
			_boxScene = ResourceLoader.Load<PackedScene>(_boxScenePath);
			if (_boxScene == null)
			{
				GD.PrintErr("Box scene cannot be found!");
			}
		}
		_box = _boxScene.Instantiate<Box>();
		_box.SetCollisionLayerValue(2, true);
		_box.SetCollisionMaskValue(1, true);
		AddChild(_box);
		_spawnedBoxes.Insert(0, _box);
		_box.GlobalPosition = spawnPosition;
	}

	public void SpawnBattery(Vector2 position)
	{
		if (_batteryScene == null)
		{
			_batteryScene = ResourceLoader.Load<PackedScene>(_batteryScenePath);
			if (_batteryScene == null)
			{
				GD.PrintErr("Battery scene cannot be found!");
			}
		}
		_battery =_batteryScene.Instantiate<Battery>();
		_battery.SetCollisionLayerValue(1, true);
		_battery.SetCollisionMaskValue(1, true);
		AddChild(_battery);
		_battery.GlobalPosition = position;
	}

	public void DestroyBattery()
	{
		if (_battery != null)
		{
			_battery.QueueFree();

			_battery = null;
		}
	}

public void ClearSpawnPoints()
{
	if (_spawnPoint != null)
		{
			_spawnPoint.QueueFree();

			_spawnPoint = null;
		}

		for (int i = 0; i < _spawnPoints.Count; i++)
		{
			_spawnPoints[i].QueueFree();
		}
		_spawnPoints.Clear();
		_spawner.ClearSpawnerList();
}

public Vector2 CreateSpawnPoints()
{
	if (_spawnPointScene == null)
		{
			_spawnPointScene = ResourceLoader.Load<PackedScene>(_spawnPointScenePath);
			if (_spawnPointScene == null)
			{
				GD.PrintErr("Box scene cannot be found!");
			}
		}
		_spawnPoint = _spawnPointScene.Instantiate<SpawnPoint>();

		AddChild(_spawnPoint);
		_spawnPoint.GlobalPosition = _spawner.GetRandomPosition();
		_spawnPoints.Insert(0, _spawnPoint);
		GD.Print(_spawnPoint);
		return _spawnPoint.GlobalPosition;
}
	public void GoToNextLevel()
	{
		_levelSceneTree = GetTree();
		_nextLevel = GD.RandRange(0 ,_levels.Count - 1);
		_spawner.fillSpawnerList(_nextLevel);
		_levelSceneTree.ChangeSceneToFile(_levels[_nextLevel]);
		Save();
	}

	public void Save()
	{
		Dictionary saveData = new Dictionary();
		if (_isGameOver)
		{
			saveData.Add("EnergyLeft", 30);
			saveData.Add("NextLevel", 0);
		}
		else
		{
		saveData.Add("EnergyLeft", _timer.GetTimer);
		saveData.Add("NextLevel", _nextLevel);
		}

		string json = Json.Stringify(saveData);
		string savePath = ProjectSettings.GlobalizePath("user://");
		savePath = Path.Combine(savePath, Config.SaveFolder);

		if (SaveToFile(savePath, Config.QuickSaveFile, json))
		{
			GD.Print("Game data saved");
		}

		else
		{
			GD.Print("1 Error saving game data");
		}
	}

	public bool Load()
	{
		string savePath = ProjectSettings.GlobalizePath("user://");
		savePath = Path.Combine(savePath, Config.SaveFolder);
		string loadedJson = LoadFromFile(savePath, Config.QuickSaveFile);

		Json jsonLoader = new Json();
		Error loadError = jsonLoader.Parse(loadedJson);
		if (loadError != Error.Ok)
		{
			GD.PrintErr($"Error while loading game. Error: {loadError}");
			return false;
		}


		Dictionary saveData = (Dictionary) jsonLoader.Data;
		_timer.GetTimer = (double)saveData["EnergyLeft"];
		_nextLevel = (int)saveData["NextLevel"];
		//score tänne
		return true;
	}
	#endregion public methods
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
public string LoadFromFile (string path, string fileName)
{
	path = Path.Combine(path, fileName);

	if (!File.Exists(path))
	{
		GD.PrintErr($"File {path} not  found");
		return null;
	}

	try
	{
		return File.ReadAllText(path);
	}

	catch (Exception e)
	{
		GD.PrintErr($"Error loading file: {e.Message}");
		return null;
	}

}
	public void GameOver()
	{
		Save();
		_isGameOver = false;
		DestroyForklift();
		DestroyBoxes();
		ClearSpawnPoints();
		_ui.ToggleGameOver();
	}
}
}