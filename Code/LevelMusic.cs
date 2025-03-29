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
                QueueFree(); // Prevent multiple instances
            }
        }

        public void PlayMusic()
        {
            if (!Playing)
            {
                Play();
            }
        }

        public void StopMusic()
        {
            Stop();
        }
    }
}