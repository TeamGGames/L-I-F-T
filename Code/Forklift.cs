using Godot;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;

namespace ForkliftGame;
public partial class Forklift : CharacterBody2D
{
	[Export] private float _steeringAngle = 50.0f; // Maximum steering angle (degrees)
	[Export] private float _enginePower = 800.0f; // Acceleration force
	[Export] private float _reversePower = -16000.0f;

	[Export] private int _gearOne = 1;

	[Export] private int _gearTwo = 1;

	[Export] private int _gearThree = 1;
	private float _wheelBase = 70.0f; // Distance from front wheel to rear wheel

	private Vector2 _velocity = Vector2.Zero;
	private float _steerAngle;
	private Vector2 _acceleration = Vector2.Zero;
	private float _friction = -0.05f;
	private float _drag = -0.0005f;

	private bool _nearBox = false;

	private List<Box> _nearBoxes;
	private List<Box> _stackedBoxes;


    public override void _Ready()
    {
		_nearBoxes = new List<Box>();
		_stackedBoxes = new List<Box>();
    }
    public override void _PhysicsProcess(double delta)
    {
		_acceleration = Vector2.Zero;
		ReadInput();
		ApplyFriction();
		CalculateSteering((float)delta);

		_velocity += _acceleration * (float)delta;
		Velocity = _velocity;
		MoveAndSlide();

    }

    public override void _Process(double delta)
    {

		//GD.Print(_stackedBoxes.Count);
		if (_nearBox)
		{
			// grabs a single nearby box
			if (Input.IsActionJustPressed("grab"))
			{
				_stackedBoxes.Insert(0, _nearBoxes[0]);
				_stackedBoxes[0].Grab();
			}
		}

		// releases all the grabbed boxes at the same time
		else if (_stackedBoxes.Count > 0  && Input.IsActionJustPressed("release"))
		//&& _stackedBoxes[0].IsGrabbed
		{
			for(int i = 0; i <= _stackedBoxes.Count; i++) {

			_stackedBoxes[0].Release();
			_stackedBoxes.Remove(_stackedBoxes[0]);
			}

		}
    }

    private void ReadInput()
	{
		float turn = 0;

		if (Input.IsActionPressed("left") && Input.IsActionPressed("gearOne"))
		{
			turn -= 1.25f;
		}

		if (Input.IsActionPressed("right")&& Input.IsActionPressed("gearOne"))
		{
			turn += 1.25f;
		}

		if(Input.IsActionPressed("right") && Input.IsActionPressed("gearTwo")) {

			turn += 1.0f;
		}

		if(Input.IsActionPressed("left") && Input.IsActionPressed("gearTwo")) {

			turn -= 1.0f;
		}

		if(Input.IsActionPressed("right") && Input.IsActionPressed("gearThree")) {

			turn += 0.75f;
		}

		if(Input.IsActionPressed("left") && Input.IsActionPressed("gearThree")) {

			turn -= 0.75f;
		}



		_steerAngle = turn * Mathf.DegToRad(_steeringAngle);


		if (Input.IsActionPressed("forward") && Input.IsActionPressed("gearOne"))
		{
			_acceleration = Transform.X * _enginePower;
		}

			else if (Input.IsActionPressed("forward") && Input.IsActionPressed("gearTwo")) {
				_acceleration = Transform.X * _enginePower * _gearTwo;
			}
			else if (Input.IsActionPressed("forward") && Input.IsActionPressed("gearThree")) {
				_acceleration = Transform.X * _enginePower * _gearThree;

				// tästä oma metodi, käytetään usein kun aloitetaan lastin tiputtelun randomointi


			// 	if (_steerAngle >=  0.5) {
			// 	//	for(int i = 0; i <= _stackedBoxes.Count; i++) {

			// 	_stackedBoxes[0].Release();
			// 	_stackedBoxes.Remove(_stackedBoxes[0]);
			// 	//}

			// }

			}

		else if (Input.IsActionPressed("reverse"))
		{

			_acceleration =  Transform.X * _reversePower;

		}
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

		// Stop when speed is low

		if (Input.IsActionJustReleased("forward")) {
			_velocity = Vector2.Zero;

		// if (_velocity.Length() < 5)
		// {
		// 	_velocity = Vector2.Zero;
		// 	return;
		}
		else if(Input.IsActionJustReleased("reverse")) {

				_velocity = Vector2.Zero;
			}



		// Apply extra friction only when moving forward.
		if (_velocity.Length() < 100 && _velocity.Dot(Transform.X) > 0)
		{
			frictionForce *= 3;
		}


		_velocity += (dragForce + frictionForce) * 0.5f;
	}

	private void OnInteractionAreaBodyEntered(Node2D body)
    {
        if (body is Box box)
        {
			_nearBox = true;
			_nearBoxes.Insert(0, box);

        }
    }

	private void OnInteractionAreaBodyExited(Node2D body)
    {
        if (body is Box box)
        {
			_nearBox = false;
			_nearBoxes.Remove(box);
        }
    }

}
