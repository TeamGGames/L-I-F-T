using Godot;
using System;
using System.Collections.Generic;

namespace ForkliftGame;
public partial class MovementSlider : VSlider
{
	private Forklift _forklift;

	private Dictionary<int, Vector2> touchData = new Dictionary<int, Vector2>();


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_forklift = GetParent().GetParent().GetParent<Forklift>();
		this.ValueChanged += OnValueChanged;
	}

    public override void _GuiInput(InputEvent @event)
    {
		if (@event is InputEventScreenTouch touch)
		{
			if (touch.IsPressed())
			{
				touchData[touch.Index] = touch.Position;
				UpdateSlider(touch.Position);
			}
			else
			{
				touchData.Remove(touch.Index);
				if ( touchData.Count == 0)
				{
					ResetSlider();
				}
			}
		}


		else if (@event is InputEventScreenDrag drag)
		{
			touchData[drag.Index] = drag.Position;
			UpdateSlider(drag.Position);
		}
    }

    public void ResetSlider()
    {
        Value = 0;
    }

    private void UpdateSlider(Vector2 position)
    {
        Value = (1-position.Y/Size.Y) * MaxValue;
    }

    private void OnValueChanged(double input)
	{
		_forklift.SpeedInput = input;
	}
}
