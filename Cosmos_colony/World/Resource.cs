namespace Space_colony_game.World
{
    /// <summary>
    /// Представляет ресурс колонии (еда, металл, энергия, люди).
    /// Содержит текущее значение, максимум, приращение за ход и порог критического уровня.
    /// </summary>
    public class Resource
    {
        /// <summary>Название ресурса (для отображения в UI).</summary>
        public string Name { get; init; } = "";

        /// <summary>Текущее значение ресурса.</summary>
        public float Value { get; set; } = 0;

        /// <summary>Максимальное значение ресурса.</summary>
        public float Max { get; set; } = 200;

        /// <summary>Изменение ресурса за последний ход (дельта).</summary>
        public float Delta { get; set; } = 0;

        /// <summary>Уровень, при котором ресурс считается критическим.</summary>
        public float CriticalLevel { get; set; } = 20;

        /// <summary>Возвращает true, если текущее значение ресурса меньше или равно критическому уровню.</summary>
        public bool IsCritical => Value <= CriticalLevel;
    }
}
