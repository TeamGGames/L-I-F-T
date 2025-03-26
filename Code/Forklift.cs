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
	// variable is used for runtime reset of the _addedWeight variable. Otherwise
	// the value _addedWeight variable would stack making the truck eventually
	// unable to move.
	private float _addedWeight;

	public float AddedWeight
	{
		get { return _addedWeight; }
		set { _addedWeight = value; }
	}
	private float _startAddedWeight = 1.0f;

	public float StartAddedWeight { get { return _startAddedWeight; } }

	// Reference to label which counts how many boxes are currently stacked and
	// carried by the forklift.
	[Export] public Label _stackedBoxesLabel = null;

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
	public List<Box> _stackedBoxes;

	private double _speedInput = 0.0;

	public double SpeedInput
	{
		get { return _speedInput; }
		set { _speedInput = value; }
	}

	public bool _leftButtonPressed = false;
	public bool _rightButtonPressed = false;
	public bool _grabReleasePressed = false;
	public bool _inArea = false;
	public bool _readForRelease = false;


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

		//ReadInput();
		ReadTouchInput();

		// Apply friction forces.

		ApplyFriction();

		// Calculate steering.

		CalculateSteering((float)delta);

		// Set _velocity.

		_velocity += _acceleration * (float)delta;

		// Velocity is a property of the forklift and used as input in MoveAndSlide() method.

		Velocity = _velocity;

		// Move the CharacterBody 2D Forklift based on the Velocity value of the forklift.
		// Could be written as "MoveAndSlide(_velocity);"
		MoveAndSlide();

    }

    public override void _Process(double delta)
    {
		// If the list _nearBox contains boxes, the user can grab it by GrabAction keypress.
		if (!_inArea)
		{
			if (_nearBox)
			{
				if (Input.IsActionJustPressed(Config.GrabAction) || _grabReleasePressed)
				{
					if ( _nearBoxes.Count > 0)
					{
						Box box = _nearBoxes[0];
						// Insert the box from _nearBoxes list to _stackedBoxes list.

						_stackedBoxes.Insert(0, box);

						// Call the Grab() method to add it as a child of the Forklift node, making it possible
						// for the forklift to carry the box.

						box.Grab();

						// Count the new value of _addedWeight. This will be used to reduce the speed of the forklift.

						_addedWeight -= _reducedSpeedFromWeight;

						// Update the amount of boxes carried shown on the label on the forklift.

						_stackedBoxesLabel.Text = $"{_stackedBoxes.Count}";
					}
				}
			}
		}

		// If there are boxes in the _stackedBoxes list, the player can release them. Will release all the boxes at once.
		if (_inArea)
		{
			if (_stackedBoxes.Count > 0  && Input.IsActionJustPressed(Config.ReleaseAction) || _stackedBoxes.Count > 0  && _grabReleasePressed && _readForRelease)
				{
					while (_stackedBoxes.Count != 0) // until the list is done
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

						_readForRelease = false;
				}

			}
		}
    }

	private void ReadTouchInput()
	{
		float turn = 0.0f;

		if (SpeedInput > 2.15 && _leftButtonPressed)
		{
			turn -= 0.5f;
		}
		else if (SpeedInput > 1.75 && _leftButtonPressed)
		{
			turn -= 0.75f;
		}
		else if (SpeedInput != 0 && _leftButtonPressed)
		{
			turn -= 1.0f;
		}
		else if (SpeedInput == 0 && _leftButtonPressed)
		{
			RotationDegrees -= 2;
		}

		if (SpeedInput > 2.15 && _rightButtonPressed)
		{
			turn += 0.5f;
		}
		else if (SpeedInput > 1.75 && _rightButtonPressed)
		{
			turn += 0.75f;
		}
		else if (SpeedInput != 0 && _rightButtonPressed)
		{
			turn += 1.0f;
		}
		else if (SpeedInput == 0 && _rightButtonPressed)
		{
			RotationDegrees += 2;
		}

		if (SpeedInput > 0)
		{
			_acceleration = Transform.X * _enginePower * (float)SpeedInput * _addedWeight;
		}
		else if (SpeedInput < 0)
		{
			_acceleration =  Transform.X * _reversePower;
		}

		if (SpeedInput > 2.25 && _rightButtonPressed || SpeedInput > 2.25 && _leftButtonPressed)
		{
			_dropBoxes = true;
		}

		_steerAngle = turn * Mathf.DegToRad(_steeringAngle);

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
			_dropBoxes = false;
		}
	}

	/// <summary>
	/// Reads user input and controls the movement of the forklift accordingly.
	/// </summary>
    private void ReadInput()
	{
		// Initialise the turn angle value of the forklift. Will be used so that the higher
		// the speed, the wider arc the forklift turns. the turn variable is set as a different
		// value depending on user input, and is either decreased or increased every frame, as well
		// as reset to zero. This gives the forklift the feel of a continuous smooth movement.

		float turn = 0;

		// On GearOne the turn arc is quite tight and boxes won't be dropped.

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

		// On GearTwo the arc is wider and the boxes won't be dropped.

		if(Input.IsActionPressed(Config.TurnRightAction) && Input.IsActionPressed(Config.GearTwo)) {

			turn += 1.0f;
			_dropBoxes = false;
		}

		if(Input.IsActionPressed(Config.TurnLeftAction) && Input.IsActionPressed(Config.GearTwo)) {

			turn -= 1.0f;
			_dropBoxes = false;
		}

		// On GearThree the arc is wide and if the player tries to turn with this speed
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

/// <summary>
/// Without CalculateSteering the forklift cannot turn. The  _steerAngle variable calculated in ReadInput()
/// is an integral part of the calulations. Also, this method provides the forklift with a natural feel to steering.
/// </summary>
/// <param name="delta"></param>
	private void CalculateSteering(float delta)
	{
		// The direction vector of both front and rear wheels are set.
		// Without using two sets of wheels the turning of the forklift would happen on a single spot,
		// conmparable to a for example a carousel.

		Vector2 rearWheel = Position - Transform.X * _wheelBase / 2.0f;
		Vector2 frontWheel = Position + Transform.X * _wheelBase / 2.0f;

		// Rear wheels won't rotate, just follow the front wheels.

		rearWheel += _velocity * delta;

		// Front wheels are rotated. NOTE: If we need to implement rear wheel rotation
		// and front wheel lock, it can be done here with a switch of frontWheel to rearWheel.

		frontWheel += _velocity.Rotated(_steerAngle) * delta;

		// The new direction vector based on the turn is calculated every frame. Also, it is
		// normalised so the actual speed of the forklift won't stack.

		Vector2 newHeading = (frontWheel - rearWheel).Normalized();

		// The new heading is set as _velocity value (Which in turn is set as Veolocity property
		// of the forklift in _Process).

		_velocity = newHeading * _velocity.Length();

		// Finally the Rotation property is set as the angle property of the newHeading vector.
		// Rotation is the forklift's rotation in relation to the level scene. That means the nose
		// of the forklift stays in the correct heading after the turn.

		Rotation = newHeading.Angle();
	}

/// <summary>
/// In applyFriction we limit Godot's RigidBody2D physics so that the forklift moves in a more
/// natural way. Without the limitations in this method there would be no slowing down when
/// for example releasing the 'forward' button. This method provides a natural feel to the
/// movement of the forklift.
/// </summary>
	private void ApplyFriction()
	{

		Vector2 frictionForce = _velocity * _friction;
		Vector2 dragForce = _velocity * _velocity.Length() * _drag;

		// Stops the forklift when going forward and the player releases the forward
		// button.

		if (Input.IsActionJustReleased(Config.ForwardAction))
		{
			_velocity = Vector2.Zero;
		}

		// Stops the forklift when reversing and and the player releases the reverse
		// button.

		else if(Input.IsActionJustReleased(Config.ReverseAction))
		{
			_velocity = Vector2.Zero;
		}

		// Apply extra friction to movement on turning the forklift. Dot calue of zero
		// is 90 degrees and considered straiaghth movement where turn friction won't apply.

		if (_velocity.Length() < 100 && _velocity.Dot(Transform.X) > 0)
		{
			frictionForce *= 3;
		}

		// Calculate the final _velocity vector based on both dragForce and fricionForce.

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

	private void EnteredLoadingArea(Area2D area)
	{
			_inArea = true;
	}

	private void ExitedLoadingArea(Area2D loadingArea)
	{
		_inArea = false;
	}
}
