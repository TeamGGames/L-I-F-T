using Godot;
using System;

namespace ForkliftGame
{
public partial class MainMenuController : Control
{

	[Export] private Button _startButton = null;
	[Export] private Button _settingsButton = null;
	[Export] private Button _instructionsButton = null;

	[Export] private DialogWindow _settingsWindow = null;
	[Export] private DialogWindow _instructionsWindow = null;


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

			_startButton.Connect(Button.SignalName.Pressed, new Callable(this, nameof(OnStartPressed)));
			_settingsButton.Connect(Button.SignalName.Pressed, new Callable(this, nameof(OnSettingsPressed)));
			_instructionsButton.Connect(Button.SignalName.Pressed, new Callable(this, nameof(OnInstructionsPressed)));

	}

	private void OnStartPressed()
	{
		GD.Print("New game pressed");
		_mainMenuSceneTree.ChangeSceneToFile("res://Levels/test_level.tscn");
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
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
}