using R3;

namespace MainMenu
{
    public class GameSettings
    {
        public ReactiveProperty<float> MusicVolue = new();
        public ReactiveProperty<float> SoundVolue = new();

        public GameSettings(GameSettingsSave save)
        {            
            MusicVolue.Value = save.MusicVolue;
            SoundVolue.Value = save.SoundVolue;
        }

        public GameSettings()
        {
            MusicVolue.Value = 1f;
            SoundVolue.Value = 1f;
        }
    }
}