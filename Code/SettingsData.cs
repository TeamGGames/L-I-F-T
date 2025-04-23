namespace ForkliftGame
{
	public class SettingsData
	{
		/// <summary>
		/// Set default values to the settings.
		/// </summary>
		/// <returns></returns>
		public static SettingsData CreateDefaults()
		{
			return new SettingsData
			{
				LangCode = "en",
				MasterVolume = -6.0f,
				MusicVolume = -6.0f,
				SfxVolume = -6.0f
			};
		}
		public string LangCode { get; set; }
		public float MasterVolume { get; set; }
		public float MusicVolume { get; set; }
		public float SfxVolume { get; set; }
	}
}