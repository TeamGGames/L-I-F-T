using Godot;
using System;

namespace ForkliftGame;
public partial class TouchControls : Control
{
	[Export] private Forklift _forklift;
	[Export] private VBoxContainer _pauseScreen;
	[Export] private Button _mainMenuButton = null;
	[Export] private Button _restartButton = null;
	[Export] private Button _resumeButton = null;

	[Export] private MainMenuController _menuController = null;
	[Export] private AudioStreamPlayer _buttonAudio = null;

	private SceneTree _mainMenuSceneTree = null;
	public bool _pause = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_mainMenuSceneTree = GetTree();
		_forklift = GetParent().GetParent<Forklift>();
		_mainMenuButton.Connect(Button.SignalName.Pressed, new Callable(this, nameof(OnMainMenuPressed)));
		_restartButton.Connect(Button.SignalName.Pressed, new Callable(this, nameof(OnRestartPressed)));
		_resumeButton.Connect(Button.SignalName.Pressed, new Callable(this, nameof(OnResumePressed)));
	}

	private void OnResumePressed()
	{
		_buttonAudio.Play();
		LevelManager.Current.Resume();
		_pause = false;
		_pauseScreen.Visible = false;
}
	private void OnMainMenuPressed()
	{
		_buttonAudio.Play();
		LevelMusic.Instance.StopMusic();
		_mainMenuSceneTree.ChangeSceneToFile("res://UI/MainMenu.tscn");
		LevelManager.Current.Resume();
		_pause = false;
		_pauseScreen.Visible = false;
	}
	private void OnRestartPressed()
	{
		_buttonAudio.Play();
		LevelMusic.Instance.StopMusic();
		LevelMusic.Instance.PlayMusic();
		_menuController.OnStartPressed();
		LevelManager.Current.Resume();
			_pause = false;
			_pauseScreen.Visible = false;
	}
	private void PauseButtonDown()
	{
		_buttonAudio.Play();
		if (!_pause) {
			LevelManager.Current.Pause();
			_pause = true;
			_pauseScreen.Visible = true;
		}
		else
		{
			LevelManager.Current.Resume();
			_pause = false;
			_pauseScreen.Visible = false;
		}

	}


	private void RightButtonDown()
	{
		_forklift._rightButtonPressed = true;
	}

	private void RightButtonUp()
	{
		_forklift._rightButtonPressed = false;
	}

	private void LeftButtonDown()
	{
		_forklift._leftButtonPressed = true;
	}

	private void LeftButtonUp()
	{
		_forklift._leftButtonPressed = false;
	}

	private void GrabReleaseButtonDown()
	{
		_forklift._grabReleasePressed = true;
	}

	private void GrabReleaseButtonUp()
	{
		_forklift._grabReleasePressed = false;
	}
	private void ReverseDown()
	{
		_forklift._reversePressed = true;
	}

	private void ReverseUp()
	{
		_forklift._reversePressed = false;
	}



}
