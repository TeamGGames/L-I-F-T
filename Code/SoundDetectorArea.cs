using Godot;
using System;

namespace ForkliftGame;

public partial class SoundDetectorArea : Area2D
{
    private TileMapLayer _shelves = null;
    [Export] private AudioStreamPlayer _collisionSound = null;
    [Export] private Forklift _forklift = null;
    public override void _Ready()
    {
        _shelves = GetNode<TileMapLayer>("../../Shelves");

    }
    /// <summary>
    /// Plays collision sound when close to shelves.
    /// </summary>
    /// <param name="body">Body</param>
    private void OnBodyEntered(Node2D body)
    {
        if (body == _shelves && _forklift.SpeedInput > 0)
        {
            _collisionSound.Play();
        }
    }
}
