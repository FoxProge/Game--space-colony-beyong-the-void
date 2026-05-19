using System.Text.Json;

namespace Space_colony_game.Core
{
    public class GameSettings
    {
        public bool Fullscreen { get; set; } = false;
        public int ScreenWidth { get; set; } = 1280;
        public int ScreenHeight { get; set; } = 720;

        private static readonly string SavePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.json");

        public static GameSettings Current { get; private set; } = new GameSettings();

        public static void Load()
        {
            if (!File.Exists(SavePath))
            {
                Current = new GameSettings();
                Save();
                return;
            }

            try
            {
                string json = File.ReadAllText(SavePath);
                Current = JsonSerializer.Deserialize<GameSettings>(json) ?? new();
            }
            catch
            {
                Current = new GameSettings();
                Save();
            }
        }

        public static void Save()
        {
            string json = JsonSerializer.Serialize(Current, new JsonSerializerOptions
            {
                WriteIndented = true    // читаемый формат
            });
            File.WriteAllText(SavePath, json);
        }
    }
}
