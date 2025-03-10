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

		private void OnBodyEntered(CharacterBody2D body)
		{
			// node on jokin toinen objekti, joka saapui tämän Area2D:n
			// alueelle.
			if (body is Forklift forklift)
			{
                GD.Print("Body entered");
				// Jos alueelle saapunut olio on forklift,
				// kerättävä esine kerätään.
				Collect(forklift);
				// Poista kerättävä esine.
				QueueFree();
			}
		}

		protected abstract void Collect(Forklift forklift);
	}
}