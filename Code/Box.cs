using Godot;
using System;


namespace ForkliftGame;
public partial class Box : RigidBody2D
{
	// The amount of points awarses per box. Adjust in Godot.
	[Export] private int _scoreAddUp = 10;

	// At the moment _isGrabbed is not used anywhere in code
	private bool _isGrabbed = false;

	// Reference to Forklift node and its CollisionShape
	// so they can be initialised in the _Ready()
	private Node2D _forklift;
	private CollisionShape2D _forkCollision;

	// Initialise the _velocity Vector of the box
	// not in use, ok to remove?
	// private Vector2 _velocity = Vector2.Zero;

	// IsGrabbed is currently not in use
	// Toggles the status if the box is grabbed
	// by the forklift
	public bool IsGrabbed {
		get { return _isGrabbed; }
	}

	// Returns the amount of points the correct-place release
	// of the box awards
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
	/// <param name="state"></param>
    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
       state.SetLinearVelocity(new Vector2 (0, 0));
	   RotationDegrees = 0;
    }

	/// <summary>
	/// Nothing here but left in place just in case it is needed later. If not, can be cleaned
	/// out before release.
	/// </summary>
	/// <param name="delta"></param>
    public override void _Process(double delta)
	{
	}

	/// <summary>
	/// When called, the forklift picks up a box and places it on the forks. After, you can drive
	/// around with the box.
	/// </summary>
	public void Grab()
	{
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

	 	// Sets the box asleep. Probably won't do much so we maybe can delete it.
		// Can't remember why I put it here.
		Sleeping = true;

	}
}
