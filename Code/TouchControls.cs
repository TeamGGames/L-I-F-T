using Godot;
using System;

namespace ForkliftGame;
public partial class TouchControls : Control
{
	[Export] private Forklift _forklift;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_forklift = GetParent().GetParent<Forklift>();
	}

	private void RightButtonDown()
	{
		_forklift._rightButtonPressed = true;
		GD.Print("right pressed");
	}

	private void RightButtonUp()
	{
		_forklift._rightButtonPressed = false;
	}

	private void LeftButtonDown()
	{
		_forklift._leftButtonPressed = true;
	}

	private void LeftButtonUp()
	{
		_forklift._leftButtonPressed = false;
	}

	private void GrabReleaseButtonDown()
	{
		_forklift._grabReleasePressed = true;
	}

	private void GrabReleaseButtonUp()
	{
		_forklift._grabReleasePressed = false;
	}
	private void ReverseDown()
	{
		_forklift._reversePressed = true;
	}

	private void ReverseUp()
	{
		_forklift._reversePressed = false;
	}



}
