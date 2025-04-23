using Godot;
using System;
using System.Reflection.Metadata.Ecma335;

namespace ForkliftGame
{


	public partial class ProgressUi : Control
	{
		[Export] private Label _score;
		[Export] private Label _highScore;

		/// <summary>
		/// Updates the current score value on ScoreLabel and shows it on screen.
		/// </summary>
		/// <param name="score">Current score</param>
		public void SetScoreLabel(int score)
		{
			_score.Text = $"{score}";

			if (LevelManager.Current.HighScore <= score)
			{
				LevelManager.Current.HighScore = score;
				SetHighScoreLabel(score);
			}
		}

		/// <summary>
		/// Updates the HighScore label on screen if the player's current score exceeds it.
		/// </summary>
		/// <param name="score">Current score</param>
		public void SetHighScoreLabel(int score)
		{
			_highScore.Text = $"{score}";
		}
	}
}