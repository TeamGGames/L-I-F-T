using Godot;
using System;

namespace ForkliftGame;
public partial class Box : RigidBody2D
{
	private bool _isGrabbed = false;
	private Node2D _forklift;
	private CollisionShape2D _forkCollision;

	public bool IsGrabbed {
		get { return _isGrabbed; }
	}
	public override void _Ready()
	{
		_forklift = GetNode<Forklift>("/root/TestLevel/Forklift");

		_forkCollision = GetNode<CollisionShape2D>(
			"/root/TestLevel/Forklift/InteractionComponents/InteractionArea/CollisionShape2D"
		);


	}

	public override void _Process(double delta)
	{
	}

	public void Grab()
	{
		GetParent().RemoveChild(this);
		_forklift.AddChild(this);
		GlobalPosition = _forkCollision.GlobalPosition;
		Freeze = true;
		_isGrabbed = true;
	}

	public void Release()
	{
		Reparent(GetTree().Root);
		Freeze = false;
		_isGrabbed = false;
	}
}
