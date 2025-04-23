using Godot;
using System;

namespace ForkliftGame
{
	public abstract partial class Collectable : Area2D
	{
		public override void _Ready()
		{
			Error connectionError = Connect(SignalName.BodyEntered,
				new Callable(this, nameof(OnBodyEntered)));

			if (connectionError != Error.Ok)
			{
				GD.PrintErr($"Error connecting BodyEntered signal: {connectionError}");
			}
		}

		/// <summary>
		/// Calls the collect method when forklift enters the collectable area.
		/// After that the collectable removes itself.
		/// </summary>
		/// <param name="body"> Body that enters the collectable's area </param>
		private void OnBodyEntered(CharacterBody2D body)
		{
			if (body is Forklift forklift)
			{
				Collect(forklift);
				QueueFree();
			}
		}

		/// <summary>
		/// Sets requirement to all children of Collectable.
		/// </summary>
		/// <param name="forklift"> Forklift is the only object that can collect. </param>
		protected abstract void Collect(Forklift forklift);
	}
}