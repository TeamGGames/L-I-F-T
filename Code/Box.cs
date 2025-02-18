using Godot;
using System;


namespace ForkliftGame;
public partial class Box : RigidBody2D
{
	[Export] private int _scoreAddUp = 10;


	public int ScoreAddUp
	{
		get { return _scoreAddUp; }
	}

	private bool _isGrabbed = false;
	private Node2D _forklift;
	private CollisionShape2D _forkCollision;
	private Vector2 _velocity = Vector2.Zero;
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

	// Don't use _PhysicsProcess because the _IntegrateForces method below is in use
    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
		//sets the box in place and it doesn't inherit the forklift's velocity
		// LinearVelocity = Vector2.Zero; won't work as elegantly
       state.SetLinearVelocity(new Vector2 (0, 0));
	   //LockRotation = true;
	   RotationDegrees = 0;

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
		Sleeping = true;

	}

	// keskeneräinen ajatus
	// public void OnBodyEntered(Node2D body)
	// {

	// 	GD.Print("wrecked box");
	// }

}