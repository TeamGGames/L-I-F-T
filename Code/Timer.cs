using Godot;
using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;


namespace ForkliftGame
{
	public partial class Timer : Node
	{
		// Timers starting time.
		[Export] private double _time = 1;
		[Export] private AudioStreamPlayer _heartBeat = null;

		// Timers current time
		private double _timer = 0;

		public double GetTimer
		{
			get { return _timer; }
			set { _timer = value; }
		}

		private bool _isRunning = false;

		private bool _isComplete = false;
		public bool IsRunning
		{
			get { return _isRunning; }
			private set { _isRunning = value; }
		}

		public bool IsComplete
		{
			get { return _isComplete; }
			private set { _isComplete = value; }
		}

		private TextureProgressBar _progressBar = null;
		private Sprite2D _progressBlood = null;
		private bool _isPlaying = false;

		public override void _Ready()
		{
			_progressBar = GetNode<TextureProgressBar>("../Forklift/UI/ProgressUI/HBoxContainer/ProgressBar");
			_progressBlood = GetNode<Sprite2D>("../Forklift/UI/ProgressUI/BloodPanel/Blood");

			if (_progressBar == null)
			{
				GD.PrintErr("Progress bar not found");
			}
			if (_progressBlood == null)
			{
				GD.PrintErr("ProgressBlood not found");
			}
		}

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
				if (_timer <= 0)
				{
					_timer = 0;
					IsComplete = true;
					Stop();
					_heartBeat.Stop();
					_isPlaying = false;

					LevelManager.Current._isGameOver = true;
					LevelManager.Current.GameOver();
				}
			}
		}

		/// <summary>
		/// Starts timer.
		/// </summary>
		public void Start()
		{
			IsRunning = true;
		}
		/// <summary>
		/// Stops timer.
		/// </summary>
		public void Stop()
		{
			IsRunning = false;
		}
		/// <summary>
		/// Sets timers time.
		/// </summary>
		/// <param name="time">time</param>
		public void SetTime(double time)
		{
			_time = time;
			_timer = _time;
		}
		/// <summary>
		/// Adds time to timer.
		/// </summary>
		/// <param name="time">time</param>
		public void AddTime(double time)
		{
			_timer = _timer + time;
			if (_timer > _time)
			{
				_timer = _time;
			}
		}
		/// <summary>
		/// Resets timer.
		/// </summary>
		/// <param name="autoStart">If true timer starts, otherwise false</param>
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