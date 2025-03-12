using Godot;
using System;


namespace ForkliftGame
{
public partial class Timer : Node
{
	// ajastimen aloitusaika
	[Export] private double _time = 1;

	// ajastimen aika tällä hetkellä
	private double _timer = 0;
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

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (IsRunning && !IsComplete)
		{
			_timer -= delta; // TODO: animaatio, akku vähenee
			//GD.Print(_timer);
			if(_timer <= 0)
			{
				//ajastin päättyi
				_timer = 0;
				IsComplete = true;
				Stop();
				GD.Print("ajastin kului loppuun");
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
		if (_timer > 15)
		{
			_timer = 15;
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