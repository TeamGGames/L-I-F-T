using System.Collections.Generic;
using Godot;
using System;

namespace ForkliftGame
{
	public class HighScore
	{
		private List<int> _scores = null;
		/// <summary>
		/// Load score data from highscore file
		/// </summary>
		/// <returns>int list of highscores </returns>
		public List<int> LoadScore()
		{
			// First check if Highscorefile exists. If it doesn't, it's created and an empty list is returned.
			if (!Godot.FileAccess.FileExists(Config.HighScoreFile))
			{
				return new List<int>();
			}
			// If the list exists, it is opened.
			using (Godot.FileAccess file = Godot.FileAccess.Open(Config.HighScoreFile, Godot.FileAccess.ModeFlags.Read))
			{
				List<int> scores = new List<int>();

				while (!file.EofReached())
				{
					// Read lines from file until the last line is reached
					string line = file.GetLine();
					string[] parts = line.Split(',');

					if (parts.Length == 2)
					{
						if (int.TryParse(parts[0], out int score))
						{
							scores.Add(score);
						}
						else
						{
							GD.PrintErr("Can't read score.");
						}
					}
					else if (!string.IsNullOrEmpty(line))
					{
						GD.PrintErr("File form not supported.");
					}
				}
				return scores;
			}
		}

		/// <summary>
		/// Getter for highscore list.
		/// </summary>
		/// <returns></returns>
		public List<int> GetScores()
		{
			if (_scores == null)
			{
				_scores = LoadScore();
			}

			return _scores;
		}

		/// <summary>
		/// Adds scores to highscore list and organises them in order.
		/// </summary>
		/// <param name="score"> the score to be entere into the highscore list.</param>
		/// <returns> true if everything works, false if error. </returns>
		public bool AddScore(int score)
		{
			if (_scores == null)
			{
				_scores = LoadScore();
			}

			_scores.Add(score);
			_scores.Sort((a, b) => b.CompareTo(a));

			while (_scores.Count > 3)
			{
				_scores.RemoveAt(_scores.Count - 1);
			}

			try
			{
				using (Godot.FileAccess file = Godot.FileAccess.Open(Config.HighScoreFile, FileAccess.ModeFlags.Write))
				{
					foreach (int scoreObject in _scores)
					{
						file.StoreLine($"{scoreObject},");
					}
				}
				return true;
			}

			catch (Exception ex)
			{
				GD.PrintErr(ex);
				return false;
			}
		}
	}
}