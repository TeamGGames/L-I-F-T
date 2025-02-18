using Godot;
using System;

namespace ForkliftGame;
public partial class LoadingArea : Node2D
{

	private Box _box;
    private int _score = 10;


	private void OnRegisteringAreaBodyEntered(Node2D body)
    {

        if (body is Box box)
        {
            _box = box;
            TestLevel.Current.Score += _box.ScoreAddUp;
            TestLevel.Current.PrintScore();

        }
    }

	private void OnRegisteringAreaBodyExited(Node2D body)
    {
        if (body is Box box && box.Freeze != true)
        {
            TestLevel.Current.Score = TestLevel.Current.Score;
            TestLevel.Current.Score -= _box.ScoreAddUp;
            _box = null;

        }
    }
}
