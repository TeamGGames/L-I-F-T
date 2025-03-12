using Godot;
using System;
using System.Collections.Generic;

namespace ForkliftGame;
public partial class LoadingArea : Node2D
/// <summary>
/// LoadingArea.cs handles the evets when a box enters or exits the loading area
/// and recognizes (updates the score) whenever the box is loaded in the
/// designated area.
/// </summary>
{

	private Box _box = null;
    private int _score = 10;
    private Vector2 _spawnPosition;
    public Vector2 SpawnPosition {
        get { return _spawnPosition; }
    }
    private List<Box> _boxesInTargetList = new List<Box>();


    public Box Box {
        get { return _box; }
    }

    public override void _Ready()
    {
        _spawnPosition = GlobalPosition;
    }

    public override void _Process(double delta)
    {
        if(_boxesInTargetList.Count >= 3)
                {
                    for (int i = 0; i < _boxesInTargetList.Count; i ++)
                    {
                        _boxesInTargetList[i].QueueFree();
                    }
                    _boxesInTargetList.Clear();
                    LevelManager.Current.GoToNextLevel();
                }
    }
/// <summary>
/// Adds points whenever a box is released int the target area.
/// </summary>
/// <param name="body"></param>
///
	private void OnRegisteringAreaBodyEntered(Node2D body)
    {

        if (body is Box box)
        {
            _box = box;
            _boxesInTargetList.Insert(0, box);
            // LevelManager.Current.Score += _box.ScoreAddUp;
            LevelManager.Current.PrintScore();
        }
    }
/// <summary>
/// Handles the box leaving the area so that no points are given when leaving.
/// Because the box becomes active (unfreezes) the moment the forklift picks it up, the
/// box exits the 2D area and points are awarded unless this piece of code is run.
/// </summary>
/// <param name="body"></param>
	// private void OnRegisteringAreaBodyExited(Node2D body)
    // {
    //     if (body is Box box && box.Freeze != true)
    //     {
    //         LevelManager.Current.Score = LevelManager.Current.Score;
    //         LevelManager.Current.Score -= _box.ScoreAddUp;
    //         _box = null;

    //     }
    // }
}
