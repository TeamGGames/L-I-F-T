using Godot;
using System;
using System.Collections.Generic;

namespace ForkliftGame;
public partial class MovementSlider : VSlider
{
	private Forklift _forklift;
	private Dictionary<int, Vector2> touchData = new Dictionary<int, Vector2>();

	public override void _Ready()
	{
		_forklift = GetParent().GetParent().GetParent<Forklift>();
		this.ValueChanged += OnValueChanged;
	}

	/// <summary>
	/// Allows multitouch on slider. Updates slider value when pressed. Resets slider value when released.
	/// </summary>
	/// <param name="event">Touch event on slider.</param>
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
				if (touchData.Count == 0)
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
	/// <summary>
	/// Resets slider to default position.
	/// </summary>
	public void ResetSlider()
	{
		Value = 0;
	}
	/// <summary>
	/// Updates slider value when called.
	/// </summary>
	/// <param name="position">Position to be assigned.</param>
	private void UpdateSlider(Vector2 position)
	{
		Value = (1 - position.Y / Size.Y) * MaxValue;
	}
	/// <summary>
	/// When slider value is changed update forklifts speedinput variable.
	/// </summary>
	/// <param name="input">Sliders current value to be updated</param>
	private void OnValueChanged(double input)
	{
		_forklift.SpeedInput = input;
	}
}
