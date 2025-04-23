using Godot;

namespace ForkliftGame
{
	public partial class PointsPowerUp : Collectable
	{
		[Export] int _pointsAdded = 10;
		ProgressUi _progressUi = null;

		/// <summary>
		/// Add points to score when collected.
		/// Show the visual effect.
		/// </summary>
		/// <param name="forklift"> Only run when forklift collects battery. </param>
		protected override void Collect(Forklift forklift)
		{
			_progressUi = GetNode<ProgressUi>("../Forklift/UI/ProgressUI");
			LevelManager.Current.CurrentScore += _pointsAdded;
			_progressUi.SetScoreLabel(LevelManager.Current.CurrentScore);
			LevelManager.Current.ShowPointsPowerUpCollectEffect();
		}
	}
}
