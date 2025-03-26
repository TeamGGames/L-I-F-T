using Godot;
using System;
using System.Reflection.Metadata.Ecma335;

namespace ForkliftGame
{


public partial class ProgressUi : Control
{
	[Export] private Label _score;
	[Export] private Label _highScore;



	public void SetScoreLabel(int score)
	{
		_score.Text = $"{score}";

		if (LevelManager.Current.HighScore <= score)
		{
			LevelManager.Current.HighScore = score;
			SetHighScoreLabel(score);
		}
	}

	public void SetHighScoreLabel(int score)
	{
		_highScore.Text = $"{score}";

	}


}
}
