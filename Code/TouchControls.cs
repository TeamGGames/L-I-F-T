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

	public override void _Ready()
	{
		_mainMenuSceneTree = GetTree();
		_forklift = GetParent().GetParent<Forklift>();
		_mainMenuButton.Connect(Button.SignalName.Pressed, new Callable(this, nameof(OnMainMenuPressed)));
		_restartButton.Connect(Button.SignalName.Pressed, new Callable(this, nameof(OnRestartPressed)));
		_resumeButton.Connect(Button.SignalName.Pressed, new Callable(this, nameof(OnResumePressed)));
	}

	/// <summary>
	/// Resumes game.
	/// </summary>
	private void OnResumePressed()
	{
		_buttonAudio.Play();
		LevelManager.Current.Resume();
		_pause = false;
		_pauseScreen.Visible = false;
	}
	/// <summary>
	/// Goes back to main menu.
	/// </summary>
	private void OnMainMenuPressed()
	{
		_buttonAudio.Play();
		LevelMusic.Instance.StopMusic();
		_mainMenuSceneTree.ChangeSceneToFile("res://UI/MainMenu.tscn");
		LevelManager.Current.Resume();
		_pause = false;
		_pauseScreen.Visible = false;
	}
	/// <summary>
	/// Restarts game.
	/// </summary>
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
	/// <summary>
	/// Pauses game or resumes it when the game is already paused.
	/// </summary>
	private void PauseButtonDown()
	{
		_buttonAudio.Play();
		if (!_pause)
		{
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
	/// <summary>
	/// Toggles boolean when button is pressed.
	/// </summary>
	private void RightButtonDown()
	{
		_forklift._rightButtonPressed = true;
	}
	/// <summary>
	/// Toggles boolean when button is released.
	/// </summary>
	private void RightButtonUp()
	{
		_forklift._rightButtonPressed = false;
	}
	/// <summary>
	/// Toggles boolean when button is pressed.
	/// </summary>
	private void LeftButtonDown()
	{
		_forklift._leftButtonPressed = true;
	}
	/// <summary>
	/// Toggles boolean when button is released.
	/// </summary>
	private void LeftButtonUp()
	{
		_forklift._leftButtonPressed = false;
	}
	/// <summary>
	/// Toggles boolean when button is pressed.
	/// </summary>
	private void GrabReleaseButtonDown()
	{
		_forklift._grabReleasePressed = true;
		_forklift._liftingSound.Play();
	}
	/// <summary>
	/// Toggles boolean when button is released.
	/// </summary>
	private void GrabReleaseButtonUp()
	{
		_forklift._grabReleasePressed = false;
	}
	/// <summary>
	/// Toggles boolean when button is pressed.
	/// </summary>
	private void ReverseDown()
	{
		_forklift._reversePressed = true;
		_forklift._backDownSound.Play();
	}
	/// <summary>
	/// Toggles boolean when button is released.
	/// </summary>
	private void ReverseUp()
	{
		_forklift._reversePressed = false;
		_forklift._backDownSound.Stop();
	}
}
