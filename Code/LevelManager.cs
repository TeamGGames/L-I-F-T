using Godot;
using System;
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
	[Export] Timer _timer = null;

	private PackedScene _forkliftScene = null;
	private PackedScene _boxScene = null;
	private PackedScene _batteryScene = null;
	private static LevelManager _current = null;

	private Forklift _forklift = null;
	private Box _box = null;
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

    public override void _Ready()
    {
        _current = this;
		// StartGame();
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
		Score = 0;
		SpawnBox();
		SpawnBattery();

	}

	public void DestroyForklift()
	{
		if (_forklift != null)
		{
			_forklift.QueueFree();
			_forklift = null;
		}
	}
	public void SpawnBox ()
	{
		if (_box != null)
		{
			_box.QueueFree();
			_box = null;
		}

		if (_boxScene == null)
		{
			_boxScene = ResourceLoader.Load<PackedScene>(_boxScenePath);
			if (_boxScene == null)
			{
				GD.PrintErr("Box scene cannot be found!");
				return;
			}
		}
		 _boxScene.Instantiate<Box>();
		 AddChild(_box);
	}

	public void SpawnBattery()
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
				return;
			}
		}
		 _batteryScene.Instantiate<Battery>();
		// AddChild(_battery);
	}


	#endregion public methods
}
}