using Godot;

namespace ForkliftGame
{
    public partial class LevelMusic : AudioStreamPlayer
    {
        public static LevelMusic Instance { get; private set; }

        public override void _Ready()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                QueueFree();
            }
        }

        /// <summary>
        /// Play music.
        /// </summary>
        public void PlayMusic()
        {
            if (!Playing)
            {
                Play();
            }
        }

        /// <summary>
        /// Stop playing music.
        /// </summary>
        public void StopMusic()
        {
            Stop();
        }
    }
}