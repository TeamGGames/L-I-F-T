using Godot;
using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;


namespace ForkliftGame
{
public partial class Timer : Node
{
	// ajastimen aloitusaika
	[Export] private double _time = 1;
	[Export] private AudioStreamPlayer _heartBeat = null;

	// ajastimen aika tällä hetkellä
	private double _timer = 0;

	public double GetTimer {
		get {return _timer;}
		set {_timer = value; }
	 	}
	//onko käynnissä vai ei
	private bool _isRunning = false;

	private bool _isComplete = false;
	public bool IsRunning {
		get {return _isRunning;}
	 	private set{_isRunning = value;}
	 	}

	public bool IsComplete {
		get{return _isComplete;}
		private set{_isComplete = value;}
		}

	private TextureProgressBar _progressBar = null;
	private TextureRect _progressBlood = null;
	private bool _isPlaying = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_progressBar = GetNode<TextureProgressBar>("../Forklift/UI/ProgressUI/HBoxContainer/ProgressBar");
		_progressBlood = GetNode<TextureRect>("../Forklift/UI/ProgressUI/Blood");

		if (_progressBar == null)
		{
			GD.PrintErr("Progress bar not found");
		}
		if (_progressBlood == null)
		{
			GD.PrintErr("ProgressBlood not found");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (IsRunning && !IsComplete)
		{
			_timer -= delta;
			_progressBar.SetValueNoSignal(_timer);

			if (_timer < 10 && !_isPlaying)
			{
				_progressBlood.Visible = true;
					_heartBeat.Play();
					_isPlaying = true;

			}
			if (_timer > 10 && _isPlaying)
			{
				_progressBlood.Visible = false;
				_heartBeat.Stop();
				_isPlaying = false;
			}
			if(_timer <= 0)
			{
				//ajastin päättyi
				_timer = 0;
				IsComplete = true;
				Stop();
				_heartBeat.Stop();
				_isPlaying = false;

				LevelManager.Current._isGameOver = true;
				LevelManager.Current.GameOver();
			}
		}
		//base._Process(delta);
	}

	// Start
	public void Start()
	{
		IsRunning = true;
	}
	// Stop

	public void Stop()
	{
		IsRunning = false;
	}

	public void SetTime(double time)
	{
		_time = time;
		_timer = _time;
		//GD.Print($"Aikaa: {_timer}");
	}

		public void AddTime(double time)
	{
		//_time = time;
		_timer = _timer + time;
		if (_timer > _time)
		{
			_timer = _time;
		}
		//GD.Print($"Aikaa lisätty {time}, aikaa jäljellä: {_timer}");
	}
	// Reset

	public void Reset(bool autoStart)
	{
		IsComplete = false;
		SetTime(_time);
		if (autoStart)
		{
			Start();
		}
		else
		{
			Stop();
		}
	}
}
}