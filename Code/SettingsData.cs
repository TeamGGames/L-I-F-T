namespace ForkliftGame
{
	public class SettingsData
	{
		public static SettingsData CreateDefaults()
		{
			return new SettingsData
			{
				MasterVolume = -6.0f,
				MusicVolume = -6.0f,
				SfxVolume = -6.0f
			};
		}

		public float MasterVolume { get; set; }
		public float MusicVolume { get; set; }
		public float SfxVolume { get; set; }
	}
}