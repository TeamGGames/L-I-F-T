using Godot;
using System;
using System.Data;

namespace ForkliftGame;
public partial class Ui : Control
{
	[Export] private Panel _gameOverPanel = null;
	[Export] private Button _quitButton = null;
	[Export] private Button _restartButton = null;
	[Export] private MainMenuController _menuController = null;
	[Export] private Label _1stScore = null;
	[Export] private Label _2ndScore = null;
	[Export] private Label _3rdScore = null;

	private SceneTree _mainMenuSceneTree = null;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
			base._Ready();
			_mainMenuSceneTree = GetTree();
			if (_mainMenuSceneTree == null)
			{
				GD.PrintErr("Scene tree not found");
			}

			_quitButton.Connect(Button.SignalName.Pressed, new Callable(this, nameof(OnQuitPressed)));
			_restartButton.Connect(Button.SignalName.Pressed, new Callable(this, nameof(OnRestartPressed)));

	}

	private void OnQuitPressed()
	{
		_mainMenuSceneTree.ChangeSceneToFile("res://UI/MainMenu.tscn");
	}
	private void OnRestartPressed()
	{
		LevelMusic.Instance.PlayMusic();
		_menuController.OnStartPressed();
	}
	public void ToggleGameOver()
	{
		_gameOverPanel.Visible = true;
	}

	public void UpdateHighScores(int score1, int score2, int score3)
	{
		if (score1 != 0)
		{
			_1stScore.Text = $"1. {score1}";
		}
		if (score2 != 0)
		{
			_2ndScore.Text = $"2. {score2}";
		}
		if (score3 != 0)
		{
			_3rdScore.Text = $"3. {score3}";
		}
	}
}
