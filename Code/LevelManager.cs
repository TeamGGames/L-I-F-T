using Godot;
using System;
using System.Collections.Generic;
using System.Data;


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
	[Export] Timer _timer = null;
	[Export] private Ui _ui = null;
	[Export] private SpawnPoint _spawner = null;

	private SceneTree _levelSceneTree = null;
	private PackedScene _forkliftScene = null;
	private PackedScene _boxScene = null;
	private PackedScene _batteryScene = null;
	private PackedScene _spawnPointScene = null;
	private static LevelManager _current = null;

	private Forklift _forklift = null;
	private Box _box = null;
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


    public override void _Ready()
    {
		_spawner.fillSpawnerList(_nextLevel);
        _current = this;
		StartGame();
		_timer.Reset(true);
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
		_forklift.GlobalPosition = _loadingArea.SpawnPosition;
		Score = 0;
		ClearSpawnPoints();

		DestroyBoxes();


		for (int i = 0; i < 3; i++)
		{
			SpawnBox(CreateSpawnPoints());

		}

		//SpawnBattery();
		//AddChild(_battery);
	}

	public void DestroyForklift()
	{
		if (_forklift != null)
		{
			_forklift.QueueFree();
			_forklift = null;
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

	public Battery SpawnBattery()
	{
		if (_battery != null)
		{
			_battery.QueueFree();
			_battery = null;
		}

		if (_batteryScene == null)
		{
			_batteryScene = ResourceLoader.Load<PackedScene>(_batteryScenePath);
			if (_batteryScene == null)
			{
				GD.PrintErr("Battery scene cannot be found!");
				return null;
			}
		}
		 return _batteryScene.Instantiate<Battery>();
		// AddChild(_battery);
	}

public void ClearSpawnPoints()
{

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

	}
	#endregion public methods

	public void GameOver()
	{
		DestroyForklift();
		DestroyBoxes();
		_ui.ToggleGameOver();
	}
}
}