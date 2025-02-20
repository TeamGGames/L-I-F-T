using Godot;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;

namespace ForkliftGame;
public partial class Forklift : CharacterBody2D
{
	// Maximum steering angle in degrees. Adjust in Godot.
	[Export] private float _steeringAngle = 50.0f;

	// Forward thrust force. The higher, the faster the forklift moves.
	[Export] private float _enginePower = 800.0f;

	// Revers thrust force. Even at this high level, the movement is
	// quite slow.
	[Export] private float _reversePower = -16000.0f;

	// The gears affect the speed of the forklift. They are multipliers,
	// the higher the gear (or multiplier), higher the speed. Adjust
	// in Godot.
	[Export] private float _gearOne = 1.0f;
	[Export] private float _gearTwo = 1.75f;
	[Export] private float _gearThree = 2.75f;

	// How much the speed of the forklift reduces when it grabs boxes.
	// Added each time a box is grabbed. Adjust in Godot.
	[Export] private float _reducedSpeedFromWeight = 0.25f;

	// Used in connection with the above _reducedSpeedFromWeight to
	// calculate thereduced speed when carrying a box. the _startAddedWeight
	// variable is used for runtime reset of the _addedWeight variable.
	private float _addedWeight;
	private float _startAddedWeight = 1.0f;

	// Reference to label which counts how many boxes are currently stacked and
	// carried by the forklift.
	[Export] private Label _stackedBoxesLabel = null;

	// Distance from front wheel to rear wheel.
	private float _wheelBase = 70.0f;

	// Initialise the _velocity of the forklift.
	private Vector2 _velocity = Vector2.Zero;

	// Initialise the _acceleration Vector2.
	private Vector2 _acceleration = Vector2.Zero;

	// Initialise the _steerAngle. Will be used in calculating the
	// turning angles of the forklift.
	private float _steerAngle;

	// _friction and _drag bot are used in controlling the speed of
	// and turning speed of the forklift.
	private float _friction = -0.05f;
	private float _drag = -0.0005f;

	// Marked true if there are boxes nearby, allowing
	// further interaction
	private bool _nearBox = false;

	// Used if the speed is too fast and making a tight angle.
	// Marked true causes the boxes to fall from the forklist.
	private bool _dropBoxes = false;

	// List of boxes to be grabbed
	private List<Box> _nearBoxes;

	// List of boxes that are currently grabbed
	private List<Box> _stackedBoxes;


    public override void _Ready()
    {
		// Initialise the lists used.
		_nearBoxes = new List<Box>();
		_stackedBoxes = new List<Box>();

		// Initialise the counter of how many boxes carried by the forklift.
		_stackedBoxesLabel.Text = $"{_stackedBoxes.Count}";

		// Initialise the _addedWeight to default value.
		_addedWeight = _startAddedWeight;
    }
    public override void _PhysicsProcess(double delta)
    {
		// Reset the _acceleration every frame so the program responds to input real-time.
		_acceleration = Vector2.Zero;

		// Read the user input.
		ReadInput();

		// Apply friction forces.
		ApplyFriction();

		// Calculate steering.
		CalculateSteering((float)delta);

		// Set _velocity.
		_velocity += _acceleration * (float)delta;
		Velocity = _velocity;

		// Move the CharacterBody 2D Forklift based on the calculated speed and direction.
		MoveAndSlide();

    }

    public override void _Process(double delta)
    {
		// If the list _nearBox contains boxes, the user can grab it by GrabAction keypress.
		if (_nearBox)
		{
			if (Input.IsActionJustPressed(Config.GrabAction))
			{
				// Insert the box from _nearBoxes list to _stackedBoxes list.

				_stackedBoxes.Insert(0, _nearBoxes[0]);

				// Call the Grab() method to add it as a child of the Forklift node, making it possible
				// for the forklift to carry the box.

				_stackedBoxes[0].Grab();

				// Count the new value of _addedWeight. This will be used to reduce the speed of the forklift.

				_addedWeight -= _reducedSpeedFromWeight;

				// Update the amount of boxes carried shown on the label on the forklift.

				_stackedBoxesLabel.Text = $"{_stackedBoxes.Count}";
			}
		}

		// If there are boxes in the _stackedBoxes list, the player can release them. Will release all the boxes at once.
		else if (_stackedBoxes.Count > 0  && Input.IsActionJustPressed(Config.ReleaseAction))
			{
				for(int i = 0; i <= _stackedBoxes.Count; i++)
			{
					// Remove the box from being a child of the forklift and restore it as a child of the scene.

					_stackedBoxes[0].Release();

					// Remove the Box from the _stackedBoxes list.

					_stackedBoxes.Remove(_stackedBoxes[0]);

					// Reset the _addedWeight so the forklift can move normally without the load. Without this, the
					// _addedWeight variable would stack until the forklift cannot move even if empty.

					_addedWeight = _startAddedWeight;

					// Update the label with the new amount of carried boxes (= zero.)

					_stackedBoxesLabel.Text = $"{_stackedBoxes.Count}";
			}

		}
    }
	/// <summary>
	/// Reads user input and controls the movement of the forklift accordingly.
	/// </summary>
    private void ReadInput()
	{
		// Initialise the turn angle value of the forklift. Will be used so that the higher
		// the speed, the wider arc the forklift turns.

		float turn = 0;

		// On Gear One the turn arc is quite tight and boxes won't be dropped.

		if (Input.IsActionPressed(Config.TurnLeftAction) && Input.IsActionPressed(Config.GearOne))
		{
			turn -= 1.25f;
			_dropBoxes = false;
		}

		if (Input.IsActionPressed(Config.TurnRightAction)&& Input.IsActionPressed(Config.GearOne))
		{
			turn += 1.25f;
			_dropBoxes = false;
		}

		// On Gear Two the arc is wider and the boxes won't be dropped.

		if(Input.IsActionPressed(Config.TurnRightAction) && Input.IsActionPressed(Config.GearTwo)) {

			turn += 1.0f;
			_dropBoxes = false;
		}

		if(Input.IsActionPressed(Config.TurnLeftAction) && Input.IsActionPressed(Config.GearTwo)) {

			turn -= 1.0f;
			_dropBoxes = false;
		}

		// On Gear Three the arc is wide and if the player tries to turn with this speed
		// the boxes will be dropped from the forklift. The player can drive straight on without
		// the boxes dropping.

		if(Input.IsActionPressed(Config.TurnRightAction) && Input.IsActionPressed(Config.GearThree)) {

			turn += 0.75f;
			_dropBoxes = true;
		}

		if(Input.IsActionPressed(Config.TurnLeftAction) && Input.IsActionPressed(Config.GearThree)) {

			turn -= 0.75f;
			_dropBoxes = true;
		}

		// On reverse the turning arc is normal and there is no fear of boxes dropping.
		if (Input.IsActionPressed(Config.ReverseAction) && Input.IsActionPressed(Config.TurnRightAction)) {
			turn = 1.0f;
			_dropBoxes = false;
		}

		if (Input.IsActionPressed(Config.ReverseAction) && Input.IsActionPressed(Config.TurnLeftAction)) {
			turn = -1.0f;
			_dropBoxes = false;
		}


		// Below the actual speed of the forklift is calculated. The if statement checks which Gear is on
		// (One, Two, Three or Reverse) and counts the _acceleration. Also the added weight from the boxes
		// is taken into account as _addedWeight.

		if (Input.IsActionPressed(Config.ForwardAction) && Input.IsActionPressed(Config.GearOne))
		{
			// _acceleration is later multiplied by delta time in _Process.
			// Transform.X is the direction, _enginePower the amount of thrust, next the
			// multiplier from _gearOne/Two/Three and finally the modifier from _addedWeight (determined
			// by how many boxes are being currently carried.)

			_acceleration = Transform.X * _enginePower * _gearOne * _addedWeight;
		}

		else if (Input.IsActionPressed(Config.ForwardAction) && Input.IsActionPressed(Config.GearTwo))
		{
			_acceleration = Transform.X * _enginePower * _gearTwo * _addedWeight;
		}

		// Gear three is a special case since boxes can fall off at this speed.

		else if (Input.IsActionPressed(Config.ForwardAction) && Input.IsActionPressed(Config.GearThree))
		{
			_acceleration = Transform.X * _enginePower * _gearThree * _addedWeight;

		// This statement  will check if _dropBoxes is set true. (Normally it is false but true only when
		// driving on Gear Three and making a turn.)

				if (_dropBoxes == true)
				{

					// Perform as long as there are items in the _stackedBoxes list.
					for(int i = 0; i < _stackedBoxes.Count; i++)
					{
					// Release a box.
					_stackedBoxes[0].Release();

					// Remove from the list.
					_stackedBoxes.Remove(_stackedBoxes[0]);

					// Update the label which tells how many boxes are carried.
					_stackedBoxesLabel.Text = $"{_stackedBoxes.Count}";

					// Reset the _addedWeight factor. Otherwise it will keep on stacking
					// and eventually the truck won't be able to move even when empty.
					_addedWeight = _startAddedWeight;
					}
				}
		}

		// Perform the same speed calculations for the reverse gear.

		if (Input.IsActionPressed(Config.ReverseAction))
		{
			_acceleration =  Transform.X * _reversePower;
		}

		// _steerAngle can now be calculated and will be used later in CalculateSteering();

		_steerAngle = turn * Mathf.DegToRad(_steeringAngle);
	}

	private void CalculateSteering(float delta)
	{
		Vector2 rearWheel = Position - Transform.X * _wheelBase / 2.0f;
		Vector2 frontWheel = Position + Transform.X * _wheelBase / 2.0f;
		rearWheel += _velocity * delta;
		frontWheel += _velocity.Rotated(_steerAngle) * delta;

		Vector2 newHeading = (frontWheel - rearWheel).Normalized();
		_velocity = newHeading * _velocity.Length();
		Rotation = newHeading.Angle();
	}

	private void ApplyFriction()
	{
		Vector2 frictionForce = _velocity * _friction;
		Vector2 dragForce = _velocity * _velocity.Length() * _drag;

		// Stops the forklift when going forward and the speed is low.

		if (Input.IsActionJustReleased(Config.ForwardAction))
		{
			_velocity = Vector2.Zero;
		}

		// Stops the forklift when reversing and the speed is low.

		else if(Input.IsActionJustReleased(Config.ReverseAction))
		{

			_velocity = Vector2.Zero;
		}

		// Apply extra friction only when moving forward.

		if (_velocity.Length() < 100 && _velocity.Dot(Transform.X) > 0)
		{
			frictionForce *= 3;
		}


		_velocity += (dragForce + frictionForce) * 0.5f;
	}

	/// <summary>
	/// If the Interactionarea of the forklift and the box meet, this method will
	/// add the box to the _nearBoxes list thus making it possible to grab and carry.
	/// </summary>
	/// <param name="body"></param>
	private void OnInteractionAreaBodyEntered(Node2D body)
    {
		// If the interaction area the enters the forklift's belongs to a box is checked first.
        if (body is Box box)
        {
			// The box gets a true value on _nearBox.

			_nearBox = true;

			// The box is inserted to the _nearBoxes list.
			_nearBoxes.Insert(0, box);
        }
    }

	/// <summary>
	/// If the interaction ares of the forklift and the box no longer overlap,
	/// this method is called and the box is removed from the _nearBoxes list.
	/// We use this so that a plpayer can't first tumble into a box and later
	/// drive further away and still be able to grab a box from distance.
	/// </summary>
	/// <param name="body"></param>
	private void OnInteractionAreaBodyExited(Node2D body)
    {	// If the interaction area the exits the forklift's belongs to a box is checked first.
        if (body is Box box)
        {
			// The box's _nearbox value is set as false.

			_nearBox = false;

			// The box is removed from the _nearBoxes list.

			_nearBoxes.Remove(box);
        }
    }
}
