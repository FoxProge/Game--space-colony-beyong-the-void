using System.Text.Json;

namespace Space_colony_game.Core
{
    /// <summary>
    /// Хранит и управляет настройками игры.
    /// </summary>
    /// <remarks>
    /// Поддерживает загрузку и сохранение в JSON-файл. Экземпляр конфигурации доступен через <see cref="Current"/>.
    /// </remarks>
    public class GameSettings
    {
        /// <summary>
        /// Режим полноэкранного отображения.
        /// </summary>
        public bool Fullscreen { get; set; } = false;

        /// <summary>
        /// Ширина окна игры в пикселях.
        /// </summary>
        public int ScreenWidth { get; set; } = 1280;

        /// <summary>
        /// Высота окна игры в пикселях.
        /// </summary>
        public int ScreenHeight { get; set; } = 720;

        /// <summary>
        /// Полный путь к файлу настроек ("Settings.json") в директории приложения.
        /// </summary>
        private static readonly string SavePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.json");

        /// <summary>
        /// Текущая загруженная конфигурация.
        /// </summary>
        /// <remarks>Свойство только для чтения извне — назначается внутри <see cref="Load"/>.</remarks>
        public static GameSettings Current { get; private set; } = new GameSettings();

        /// <summary>
        /// Загружает настройки из файла.
        /// </summary>
        /// <remarks>
        /// Если файл отсутствует, либо при ошибке чтения/десериализации — создаются значения по умолчанию и они сохраняются.
        /// Метод безопасно обрабатывает ошибки при чтении/десериализации и гарантирует, что <see cref="Current"/> будет иметь корректный объект настроек.
        /// </remarks>
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
                // В случае любой ошибки при чтении/десериализации используем значения по умолчанию и сохраняем их.
                Current = new GameSettings();
                Save();
            }
        }

        /// <summary>
        /// Сохраняет текущие настройки в файл в формате JSON с отступами.
        /// </summary>
        /// <remarks>
        /// Метод выполняет запись на диск с помощью <see cref="File.WriteAllText(string, string)"/>.
        /// </remarks>
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
