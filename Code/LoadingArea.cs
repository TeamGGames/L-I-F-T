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
    private SpawnPoint _spawner = null;
    private int _score = 10;
    private Vector2 _spawnPosition;
    public Vector2 SpawnPosition {
        get { return _spawnPosition; }
    }
    private List<Box> _boxesInTargetList = new List<Box>();
    private ProgressUi _progressUi = null;


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
            if (_spawner == null)
            {
                _spawner = GetNode<SpawnPoint>("../SpawnPoint");
            }
            _box = box;
            _box.GlobalPosition = _spawner._boxOnTruckList[0];
            _spawner._boxOnTruckList.RemoveAt(0);
            _boxesInTargetList.Insert(0, box);
            LevelManager.Current.CurrentScore += _box.ScoreAddUp;
            _progressUi = GetNode<ProgressUi>("../Forklift/UI/ProgressUI");
            _progressUi.SetScoreLabel(LevelManager.Current.CurrentScore);

        }

        if (body is Forklift forklift)
        {
            forklift._readForRelease = true;
        }
    }
}
