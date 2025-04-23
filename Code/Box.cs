using Godot;
using System;


namespace ForkliftGame;
public partial class Box : RigidBody2D
{

	[Export] private int _scoreAddUp = 10;
	[Export] private PointLight2D _glow = null;
	private bool _isGrabbed = false;
	private Node2D _forklift;
	private CollisionShape2D _forkCollision;
	public int ScoreAddUp
	{
		get { return _scoreAddUp; }
	}
	public override void _Ready()
	{
		_forklift = GetNode<Forklift>("/root/TestLevel/Forklift");
		_forkCollision = GetNode<CollisionShape2D>(
			"/root/TestLevel/Forklift/InteractionComponents/InteractionArea/CollisionShape2D"
		);
	}

	/// <summary>
	/// Don't use _PhysicsProcess because the _IntegrateForces method below is in use.
	/// Sets the box in place and it doesn't inherit the forklift's velocity.
	/// "LinearVelocity = Vector2.Zero;" won't work as elegantly. With this code the box is picked up neatly
	/// and also it won't rotate when released from the forklift.
	/// </summary>
	/// <param name="state"> current physics state of the box. </param>
	public override void _IntegrateForces(PhysicsDirectBodyState2D state)
	{
		state.SetLinearVelocity(new Vector2(0, 0));
		RotationDegrees = 0;
	}

	/// <summary>
	/// When called, the forklift picks up a box and places it on the forks. After, you can drive
	/// around with the box.
	/// </summary>
	public void Grab()
	{
		_glow.Visible = false;
		// Remove the box from its parent
		GetParent().RemoveChild(this);

		// Add the box as a child of the forklift
		_forklift.AddChild(this);

		// Set the box's position as the position of the new parent's collisionshape
		GlobalPosition = _forkCollision.GlobalPosition;

		// Remove physics from box
		Freeze = true;

		// Set the box as having been grabbed
		_isGrabbed = true;
	}

	/// <summary>
	/// When called, the box is released from the forklift and it regains its own physics
	/// </summary>
	public void Release()
	{
		// The box is reparented to its original tree defined in the Godot Scene
		Reparent(GetTree().Root);

		// The box regains its own physics
		Freeze = false;

		// The status is set as not grabbed by the boolean
		_isGrabbed = false;
	}
}