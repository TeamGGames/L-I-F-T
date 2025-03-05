using Godot;
using System;
using System.Data;

namespace ForkliftGame;


/// <summary>
/// Currently the Testlevel.cs handles keeping track of the score
/// </summary>
public partial class TestLevel : Node2D
{

	[Export] Timer _timer = null;
	private static TestLevel _current = null;
			public static TestLevel Current
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
		_timer.Reset(true);
		//_timer.AddTime(3.0); //aktivoidaan kun saadaan signaali collectablelta ajan lisäksestä

    }


    public void PrintScore()
	{
		GD.Print($"Score: {Score}");
	}
}
