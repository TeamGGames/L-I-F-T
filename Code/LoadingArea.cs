using Godot;
using System;

namespace ForkliftGame;
public partial class LoadingArea : Node2D
/// <summary>
/// LoadingArea.cs handles the evets when a box enters or exits the loading area
/// and recognizes (updates the score) whenever the box is loaded in the
/// designated area.
/// </summary>


{

	private Box _box;
    private int _score = 10;

/// <summary>
/// Adds points whenever a box is released int the target area.
/// </summary>
/// <param name="body"></param>
	private void OnRegisteringAreaBodyEntered(Node2D body)
    {

        if (body is Box box)
        {
            _box = box;
            TestLevel.Current.Score += _box.ScoreAddUp;
            TestLevel.Current.PrintScore();

        }
    }
/// <summary>
/// Handles the box leaving the area so that no points are given when leaving.
/// Because the box becomes active (unfreezes) the moment the forklift picks it up, the
/// box exits the 2D area and points are awarded unless this piece of code is run.
/// </summary>
/// <param name="body"></param>
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
