using Godot;
using System;

namespace ForkliftGame;
public partial class Ui : Control
{
	[Export] private VBoxContainer _gameOverWindow = null;
	[Export] private Button _quitButton = null;

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

	}

	private void OnQuitPressed()
	{
		GD.Print("Returning to main menu");
		_mainMenuSceneTree.ChangeSceneToFile("res://UI/MainMenu.tscn");
	}
	public void ToggleGameOver()
	{
		_gameOverWindow.Visible = true;
	}
}
