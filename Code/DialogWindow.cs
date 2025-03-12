using System;
using Godot;

namespace ForkliftGame
{
	public partial class DialogWindow : Control
	{
		[Export] private Label _headlineLabel = null;
		[Export] private Label _textLabel = null;
		[Export] private Button _okButton = null;
		[Export] private Button _cancelButton = null;

		[Export] private string _defaultHeadline = "Dialog";
		[Export] private string _defaultText = "This is a dialog.";

		[Export] private bool _openOnStart = true;

		public override void _Ready()
		{
			if (_okButton != null)
			{
				_okButton.Connect(Button.SignalName.Pressed,
					new Callable(this, nameof(OnOkButtonPressed)));
			}

			if (_cancelButton != null)
			{
				_cancelButton.Connect(Button.SignalName.Pressed,
					new Callable(this, nameof(OnCancelButtonPressed)));
			}

			if (!String.IsNullOrEmpty(_defaultHeadline))
			{
				SetHeadline(_defaultHeadline);
			}

			if (!String.IsNullOrEmpty(_defaultText))
			{
				SetText(_defaultText);
			}

			if (_openOnStart)
			{
				Open();
			}
			else
			{
				Close();
			}
		}

		public virtual void Open()
		{
			Show();
		}

		public virtual void Close()
		{
			Hide();
		}

		public void SetHeadline(string text)
		{
			if (_headlineLabel != null)
			{
				_headlineLabel.Text = text;
			}
		}

		public void SetText(string text)
		{
			if (_textLabel != null)
			{
				_textLabel.Text = text;
			}
		}

		protected virtual void OnOkButtonPressed()
		{
			// Handle OK button press logic here
			Close();
		}

		protected virtual void OnCancelButtonPressed()
		{
			// Handle Cancel button press logic here
			Close();
		}
	}
}