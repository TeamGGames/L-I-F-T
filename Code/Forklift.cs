using Godot;
using System;
using System.Xml.Serialization;

namespace Forklift;
public partial class Forklift : CharacterBody2D
{
	[Export] private float _steeringAngle = 50.0f; // Maximum steering angle (degrees)
	[Export] private float _enginePower = 800.0f; // Acceleration force
	private float _wheelBase = 70.0f; // Distance from front wheel to rear wheel

	private Vector2 _velocity = Vector2.Zero;
	private float _steerAngle;
	private Vector2 _acceleration = Vector2.Zero;
	private float _friction = -0.05f;
	private float _drag = -0.0005f;

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

	private void ReadInput()
	{
		int turn = 0;

		if (Input.IsActionPressed("left"))
		{
			turn -= 1;
		}

		if (Input.IsActionPressed("right"))
		{
			turn += 1;
		}

		_steerAngle = turn * Mathf.DegToRad(_steeringAngle);

		if (Input.IsActionPressed("forward"))
		{
			_acceleration = Transform.X * _enginePower;
		} else if (Input.IsActionPressed("reverse"))
		{
			_acceleration = -Transform.X * _enginePower;
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
		// Stop when speed is low
		if (_velocity.Length() < 5)
		{
			_velocity = Vector2.Zero;
			return;
		}

		Vector2 frictionForce = _velocity * _friction;
		Vector2 dragForce = _velocity * _velocity.Length() * _drag;

		// Apply extra friction only when moving forward.
		if (_velocity.Length() < 100 && _velocity.Dot(Transform.X) > 0)
		{
			frictionForce *= 3;
		}

		_velocity += (dragForce + frictionForce) * 0.5f;
	}
}
