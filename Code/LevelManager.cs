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
		[Export] private string _loadingAreaScenePath = "res://Levels/loading_area.tscn";
		[Export] private string _pointsPowerUpScenePath = "res://GameComponents/PointsPowerUp.tscn";
		[Export] private PackedScene _collectBatteryEffectScene = null;
		[Export] private PackedScene _collectPointsPowerUpEffectScene = null;
		[Export] private Ui _ui = null;
		[Export] private SpawnPoint _spawner = null;
		[Export] private Node2D _signVariant1 = null;
		[Export] private Node2D _signVariant2 = null;
		[Export] private Node2D _signVariant3 = null;
		[Export] private int _addedScore = 500;
		private List<Node2D> _signVariantList = new List<Node2D>();
		private SceneTree _levelSceneTree = null;
		private PackedScene _forkliftScene = null;
		private PackedScene _boxScene = null;
		private PackedScene _batteryScene = null;
		private PackedScene _spawnPointScene = null;
		private PackedScene _timerScene = null;
		private PackedScene _pointsPowerUpScene = null;
		private PackedScene _loadingAreaScene = null;
		private static LevelManager _current = null;
		private GpuParticles2D _collectBatteryEffect = null;
		private GpuParticles2D _collectPointsPowerUpEffect = null;
		private Forklift _forklift = null;
		private Box _box = null;
		private Timer _timer = null;
		private SpawnPoint _spawnPoint = null;
		private LoadingArea _loadingArea = null;
		private Battery _battery = null;
		private MovementSlider _movementSlider = null;
		private PointsPowerUp _pointsPowerUp = null;
		public static LevelManager Current
		{
			get { return _current; }
		}
		private int _highScore = 0;
		private int _currentScore = 0;
		public int CurrentScore
		{
			get { return _currentScore; }
			set { _currentScore = value; }
		}
		public int HighScore
		{
			get { return _highScore; }
			set { _highScore = value; }
		}
		private List<Box> _spawnedBoxes = new List<Box>();
		private List<SpawnPoint> _spawnPoints = new List<SpawnPoint>();
		private List<SpawnPoint> _loadingAreaPoints = new List<SpawnPoint>();
		private List<string> _levels = new List<string> { Config.Level0, Config.Level1, Config.Level2, Config.Level3 };
		public int _nextLevel = 0;
		public bool _isGameOver = false;
		private bool _batteryAdded = false;
		private ProgressUi _progressUi = null;
		private List<int> _highScoreList = new List<int>();
		private double _maxEnergy;

		public override void _Ready()
		{
			_current = this;
			_signVariantList.Add(_signVariant1);
			_signVariantList.Add(_signVariant2);
			_signVariantList.Add(_signVariant3);
			StartGame();
			_collectBatteryEffect = _collectBatteryEffectScene.Instantiate<GpuParticles2D>();
			_collectPointsPowerUpEffect = _collectPointsPowerUpEffectScene.Instantiate<GpuParticles2D>();
		}

		/// <summary>
		/// Initialize the level and first destroy all previous objects (if any) and then spawns all current level objects.
		/// </summary>
		public void StartGame()
		{
			DestroyForklift();
			_forklift = CreateForklift();
			AddChild(_forklift);

			_movementSlider = GetNode<MovementSlider>("Forklift/UI/TouchControls/MovementSlider");
			_movementSlider.ResetSlider();

			DestroyTimer();
			_timer = CreateTimer();
			AddChild(_timer);
			_maxEnergy = _timer.GetTimer;
			_timer.Reset(true);

			if (!Load())
			{
				GD.PrintErr("Load error");
			}

			DestroyLoadingArea();
			_spawner.fillLoadingAreaSpawnerList(_nextLevel);
			CreateLoadingArea(CreateLoadingAreaSpawnPoint());

			_progressUi = GetNode<ProgressUi>("Forklift/UI/ProgressUI");

			HighScore highScore = new HighScore();
			_highScoreList = highScore.GetScores();
			if (_highScoreList.Count != 0)
			{
				_highScore = _highScoreList[0];
				_progressUi.SetHighScoreLabel(_highScore);
			}

			_spawner.fillSpawnerList(_nextLevel);

			_forklift.GlobalPosition = _loadingArea.GlobalPosition;

			_progressUi.SetScoreLabel(LevelManager.Current.CurrentScore);

			DestroyBoxes();
			for (int i = 0; i < 3; i++)
			{
				SpawnBox(CreateSpawnPoints());
			}

			DestroyBattery();
			DestroyPointsPowerUp();
			for (int i = 0; i < 2; i++)
			{
				SpawnBattery(CreateSpawnPoints());
			}
			SpawnPointsPowerUp(CreateSpawnPoints());

			_signVariantList[_spawner.randomLoadingAreaIndex].Visible = true;
			_spawner.fillTruckSpawnerList(_nextLevel, _spawner.randomLoadingAreaIndex);
		}

		/// <summary>
		/// Create forklift and add it to the current level scene.
		/// </summary>
		/// <returns> Forklift object. </returns>
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

		/// <summary>
		/// Create Loading Area add it to the current level scene.
		/// </summary>
		/// <returns> Loading Area object. </returns>
		private void CreateLoadingArea(Vector2 spawnPosition)
		{
			if (_loadingAreaScene == null)
			{
				_loadingAreaScene = ResourceLoader.Load<PackedScene>(_loadingAreaScenePath);
				if (_loadingAreaScene == null)
				{
					GD.PrintErr("Box scene cannot be found!");
				}
			}
			_loadingArea = _loadingAreaScene.Instantiate<LoadingArea>();
			AddChild(_loadingArea);
			_loadingArea.GlobalPosition = spawnPosition;
		}

		/// <summary>
		/// Create Timer add it to the current level scene.
		/// </summary>
		/// <returns> Timer object. </returns>
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

		/// <summary>
		/// If loading area exists, remove it from the scene and clear references to it.
		/// </summary>
		private void DestroyLoadingArea()
		{
			if (_loadingArea != null)
			{
				_loadingArea.QueueFree();
				_loadingArea = null;
			}
			if (_loadingAreaPoints.Count > 0)
			{
				_loadingAreaPoints[0].QueueFree();
			}
			_loadingAreaPoints.Clear();
		}
		/// <summary>
		/// Create a free spwn point for the loading area and set the loading area
		/// object to its coordinate.
		/// </summary>
		/// <returns> Vector2 coordinate  </returns>
		private Vector2 CreateLoadingAreaSpawnPoint()
		{
			if (_spawnPointScene == null)
			{
				_spawnPointScene = ResourceLoader.Load<PackedScene>(_spawnPointScenePath);
				if (_spawnPointScene == null)
				{
					GD.PrintErr("SpawnPoint scene cannot be found!");
				}
			}
			_spawnPoint = _spawnPointScene.Instantiate<SpawnPoint>();
			AddChild(_spawnPoint);
			_spawnPoint.GlobalPosition = _spawner.GetRandomLoadingAreaPosition();
			_loadingAreaPoints.Insert(0, _spawnPoint);
			return _spawnPoint.GlobalPosition;
		}
		/// <summary>
		/// If Forklift exists, remove it from the scene and clear references to it.
		/// </summary>
		public void DestroyForklift()
		{
			if (_forklift != null)
			{
				_forklift.QueueFree();
				_forklift = null;
			}
		}

		/// <summary>
		/// If Timer exists, remove it from the scene and clear references to it.
		/// </summary>
		public void DestroyTimer()
		{
			if (_timer != null)
			{
				_timer.QueueFree();
				_timer = null;
			}
		}

		/// <summary>
		/// If boxes exist, remove them from the scene and clear references.
		/// </summary>
		public void DestroyBoxes()
		{
			for (int i = 0; i < _spawnedBoxes.Count; i++)
			{
				_spawnedBoxes[i].QueueFree();
			}
			_spawnedBoxes.Clear();

			if (_box != null)
			{
				_box.QueueFree();
				_box = null;
			}
		}
		/// <summary>
		/// Adds boxes to the current scene.
		/// </summary>
		/// <param name="spawnPosition"> Vector2 coordinate in which the box is placed. </param>
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

		/// <summary>
		/// Adds Battery powerup to the current scene.
		/// </summary>
		/// <param name="position"> Vector2 coordinate in which the battery is placed.</param>
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
			_battery = _batteryScene.Instantiate<Battery>();
			_battery.SetCollisionLayerValue(1, true);
			_battery.SetCollisionMaskValue(1, true);
			AddChild(_battery);
			_battery.GlobalPosition = position;
		}

		/// <summary>
		/// Remove battery and references to it from the current scene.
		/// </summary>
		public void DestroyBattery()
		{
			if (_battery != null)
			{
				_battery.QueueFree();
				_battery = null;
			}
		}
		/// <summary>
		/// Adds Points powerup to the current scene.
		/// </summary>
		/// <param name="position">Vector2 coordinate in which the Points powerup is placed.</param>
		public void SpawnPointsPowerUp(Vector2 position)
		{
			if (_pointsPowerUpScene == null)
			{
				_pointsPowerUpScene = ResourceLoader.Load<PackedScene>(_pointsPowerUpScenePath);
				if (_pointsPowerUpScene == null)
				{
					GD.PrintErr("PointsPowerUp scene cannot be found!");
				}
			}
			_pointsPowerUp = _pointsPowerUpScene.Instantiate<PointsPowerUp>();
			_pointsPowerUp.SetCollisionLayerValue(1, true);
			_pointsPowerUp.SetCollisionMaskValue(1, true);
			AddChild(_pointsPowerUp);
			_pointsPowerUp.GlobalPosition = position;
		}

		/// <summary>
		/// Remove Points powerup and refernces to it from the current scene.
		/// </summary>
		public void DestroyPointsPowerUp()
		{
			if (_pointsPowerUp != null)
			{
				_pointsPowerUp.QueueFree();
				_pointsPowerUp = null;
			}
		}
		/// <summary>
		/// Remove spawn points from the scene and empty spawn point list.
		/// </summary>
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
			_spawner.ClearLoadingAreaSpawnerList();
			_spawner._boxOnTruckList.Clear();
		}

		/// <summary>
		/// Create and add spawn points to the current scene.
		/// </summary>
		/// <returns></returns>
		public Vector2 CreateSpawnPoints()
		{
			if (_spawnPointScene == null)
			{
				_spawnPointScene = ResourceLoader.Load<PackedScene>(_spawnPointScenePath);
				if (_spawnPointScene == null)
				{
					GD.PrintErr("Spawn Point scene cannot be found!");
				}
			}
			_spawnPoint = _spawnPointScene.Instantiate<SpawnPoint>();
			AddChild(_spawnPoint);
			_spawnPoint.GlobalPosition = _spawner.GetRandomPosition();
			_spawnPoints.Insert(0, _spawnPoint);
			return _spawnPoint.GlobalPosition;
		}

		/// <summary>
		/// Randomizes level and changes the scene to it.
		/// </summary>
		public void GoToNextLevel()
		{
			CurrentScore += _addedScore;
			_levelSceneTree = GetTree();
			_nextLevel = GD.RandRange(1, _levels.Count - 1);
			_spawner.fillSpawnerList(_nextLevel);
			_levelSceneTree.ChangeSceneToFile(_levels[_nextLevel]);
			Save();
		}

		/// <summary>
		/// Saves current player data to external file.
		/// </summary>
		public void Save()
		{
			Godot.Collections.Dictionary saveData = new Godot.Collections.Dictionary();
			if (_isGameOver)
			{
				saveData.Add("EnergyLeft", _maxEnergy);
				saveData.Add("NextLevel", 0);
				saveData.Add("Score", 0);
			}
			else
			{
				saveData.Add("EnergyLeft", _timer.GetTimer);
				saveData.Add("NextLevel", _nextLevel);
				saveData.Add("Score", _currentScore);
			}

			string json = Json.Stringify(saveData);
			string savePath = ProjectSettings.GlobalizePath("user://");
			savePath = Path.Combine(savePath, Config.SaveFolder);

			if (!SaveToFile(savePath, Config.QuickSaveFile, json))
			{
				GD.PrintErr("Error saving game data");
			}
		}

		/// <summary>
		/// Reads and assigns data to variables.
		/// </summary>
		/// <returns> true if no errors. </returns>
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

			Godot.Collections.Dictionary saveData = (Godot.Collections.Dictionary)jsonLoader.Data;
			_timer.GetTimer = (double)saveData["EnergyLeft"];
			_nextLevel = (int)saveData["NextLevel"];
			_currentScore = (int)saveData["Score"];
			return true;
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
		/// Reads the save file.
		/// </summary>
		/// <param name="path"> Path to the save file.</param>
		/// <param name="fileName"> Name of the save file.</param>
		/// <returns> save data in a string format</returns>
		public string LoadFromFile(string path, string fileName)
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

		/// <summary>
		/// Shows battery collect effect on forklift's position.
		/// </summary>
		/// <returns> true, if succesful </returns>
		public bool ShowBatteryCollectEffect()
		{
			if (_collectBatteryEffectScene != null && !_batteryAdded)
			{
				AddChild(_collectBatteryEffect);
				_batteryAdded = true;
			}
			else if (_collectBatteryEffect == null && _collectBatteryEffectScene == null)
			{
				GD.PrintErr("Collect effect scene not loaded!");
				return false;
			}

			_collectBatteryEffect.Position = _forklift.GlobalPosition;
			_collectBatteryEffect.Restart();
			_collectBatteryEffect.OneShot = true;
			return true;
		}

		/// <summary>
		/// Shows points powerup collect effect on forklift's position.
		/// </summary>
		/// <returns> true, if succesful </returns>
		public bool ShowPointsPowerUpCollectEffect()
		{
			if (_collectPointsPowerUpEffectScene != null)
			{

				AddChild(_collectPointsPowerUpEffect);
			}
			else if (_collectPointsPowerUpEffect == null && _collectPointsPowerUpEffectScene == null)
			{
				GD.PrintErr("Collect effect scene not loaded!");
				return false;
			}

			_collectPointsPowerUpEffect.Position = _forklift.GlobalPosition;
			_collectPointsPowerUpEffect.Restart();
			_collectPointsPowerUpEffect.OneShot = true;
			return true;
		}

		/// <summary>
		/// Pause game
		/// </summary>
		public void Pause()
		{
			if (_levelSceneTree == null)
			{
				_levelSceneTree = GetTree();
			}
			Engine.TimeScale = 0;
		}

		/// <summary>
		/// Resume play.
		/// </summary>
		public void Resume()
		{
			if (_levelSceneTree == null)
			{
				_levelSceneTree = GetTree();
			}
			Engine.TimeScale = 1;
		}

		/// <summary>
		/// Shows the game over screen, saves the game and updates highscore list.
		/// </summary>

		public void GameOver()
		{
			LevelMusic.Instance.StopMusic();
			_ui.PlayGameOverSound();
			_signVariantList[_spawner.randomLoadingAreaIndex].Visible = false;
			int csvScore1 = 0;
			int csvScore2 = 0;
			int csvScore3 = 0;
			Save();

			HighScore highScore = new HighScore();
			highScore.AddScore(_currentScore);
			List<int> scores = highScore.LoadScore();
			if (scores == null)
			{
				GD.PrintErr("BIG mistake!!!");
			}
			else
			{
				csvScore1 = scores[0];
				csvScore2 = scores[1];
				csvScore3 = scores[2];
			}

			_ui.UpdateHighScores(csvScore1, csvScore2, csvScore3);
			_isGameOver = false;
			DestroyForklift();
			DestroyBoxes();
			ClearSpawnPoints();
			_ui.ToggleGameOver();
		}
	}
}