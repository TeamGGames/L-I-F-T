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

	[Export] private float _gearOne = 1.0f;

	[Export] private float _gearTwo = 1.75f;

	[Export] private float _gearThree = 2.75f;
	[Export] private float _reducedSpeedFromWeight = 0.15f;
	[Export] private Label _stackedBoxesLabel = null;
	private float _wheelBase = 70.0f; // Distance from front wheel to rear wheel

	private Vector2 _velocity = Vector2.Zero;
	private float _steerAngle;

	private float _addedWeight = 1.0f;

	private Vector2 _acceleration = Vector2.Zero;
	private float _friction = -0.05f;
	private float _drag = -0.0005f;

	private bool _nearBox = false;
	private bool _dropBoxes = false;
	private List<Box> _nearBoxes;
	private List<Box> _stackedBoxes;


    public override void _Ready()
    {
		_nearBoxes = new List<Box>();
		_stackedBoxes = new List<Box>();
		_stackedBoxesLabel.Text = $"{_stackedBoxes.Count}";
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
			if (Input.IsActionJustPressed(Config.GrabAction))
			{
				_stackedBoxes.Insert(0, _nearBoxes[0]);
				_stackedBoxes[0].Grab();
				_addedWeight -= _reducedSpeedFromWeight;
				_stackedBoxesLabel.Text = $"{_stackedBoxes.Count}";
			}
		}

		// releases all the grabbed boxes at the same time
		else if (_stackedBoxes.Count > 0  && Input.IsActionJustPressed(Config.ReleaseAction))
		//&& _stackedBoxes[0].IsGrabbed
		{
			for(int i = 0; i <= _stackedBoxes.Count; i++) {

			_stackedBoxes[0].Release();
			_stackedBoxes.Remove(_stackedBoxes[0]);
			_addedWeight = 1.0f;
			_stackedBoxesLabel.Text = $"{_stackedBoxes.Count}";
			}

		}
    }

    private void ReadInput()
	{
		float turn = 0;

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

		if(Input.IsActionPressed(Config.TurnRightAction) && Input.IsActionPressed(Config.GearTwo)) {

			turn += 1.0f;
			_dropBoxes = false;
		}

		if(Input.IsActionPressed(Config.TurnLeftAction) && Input.IsActionPressed(Config.GearTwo)) {

			turn -= 1.0f;
			_dropBoxes = false;
		}

		if(Input.IsActionPressed(Config.TurnRightAction) && Input.IsActionPressed(Config.GearThree)) {

			turn += 0.75f;
			_dropBoxes = true;
		}

		if(Input.IsActionPressed(Config.TurnLeftAction) && Input.IsActionPressed(Config.GearThree)) {

			turn -= 0.75f;
			_dropBoxes = true;
		}

		if (Input.IsActionPressed(Config.ReverseAction) && Input.IsActionPressed(Config.TurnRightAction)) {
			turn = 1.0f;
			_dropBoxes = false;
		}

		if (Input.IsActionPressed(Config.ReverseAction) && Input.IsActionPressed(Config.TurnLeftAction)) {
			turn = -1.0f;
			_dropBoxes = false;
		}



		_steerAngle = turn * Mathf.DegToRad(_steeringAngle);
		//GD.Print($"steer angle: {_steerAngle}");
		//GD.Print($"steering angle: {_steeringAngle}");

		if (Input.IsActionPressed(Config.ForwardAction) && Input.IsActionPressed(Config.GearOne))
		{
			_acceleration = Transform.X * _enginePower * _addedWeight;
		}

		else if (Input.IsActionPressed(Config.ForwardAction) && Input.IsActionPressed(Config.GearTwo)) {
				_acceleration = Transform.X * _enginePower * _gearTwo * _addedWeight;
		}
		else if (Input.IsActionPressed(Config.ForwardAction) && Input.IsActionPressed(Config.GearThree)) {
				_acceleration = Transform.X * _enginePower * _gearThree * _addedWeight;

				// tästä oma metodi, käytetään usein kun aloitetaan lastin tiputtelun randomointi
				// kun käytetään _stackedBoxesFallBreakPoint -muuttujaa niin voidaan määrittää missä
				// vaiheessa ruvetaan kaatamaan pinoa trukin kyydistä
				// TODO: jos

				if (_dropBoxes == true) {
					for(int i = 0; i < _stackedBoxes.Count; i++) {
					_stackedBoxes[0].Release();
					_stackedBoxes.Remove(_stackedBoxes[0]);
					}

				}

		}

		if (Input.IsActionPressed(Config.ReverseAction))
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
		if (Input.IsActionJustReleased(Config.ForwardAction)) {
			_velocity = Vector2.Zero;
		}
		else if(Input.IsActionJustReleased(Config.ReverseAction)) {

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
