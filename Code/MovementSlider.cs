using Godot;
using System;

namespace ForkliftGame;
public partial class MovementSlider : VSlider
{
	[Export] private Forklift _forklift;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_forklift = GetParent().GetParent().GetParent<Forklift>();
		this.ValueChanged += OnValueChanged;
	}

	private void OnValueChanged(double input)
	{
		_forklift.SpeedInput = input;
	}
}
