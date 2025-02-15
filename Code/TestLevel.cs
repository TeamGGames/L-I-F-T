using Godot;
using System;

namespace ForkliftGame;
public partial class TestLevel : Node2D
{
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
    }


    public void PrintScore()
	{
		GD.Print($"Score: {Score}");
	}
}
