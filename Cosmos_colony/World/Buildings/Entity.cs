namespace Space_colony_game.World.Buildings
{
    /// <summary>
    /// Базовый абстрактный класс для всех сущностей/объектов на карте (зданий и подобных).
    /// Определяет идентификатор, позицию в тайлах и контракт на ежедневное обновление и отрисовку.
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// Уникальный идентификатор сущности.
        /// Инициализируется при создании и недоступен для изменения после инициализации.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Отображаемое имя сущности (по умолчанию пустая строка).
        /// </summary>
        public string Name { get; init; } = "";

        /// <summary>
        /// Колонка (x) на карте в тайлах.
        /// </summary>
        public int Col { get; init; }

        /// <summary>
        /// Строка (y) на карте в тайлах.
        /// </summary>
        public int Row { get; init; }

        /// <summary>
        /// Вызывается при переходе дня — позволяет сущности применить ежедневные эффекты/логики.
        /// </summary>
        public abstract void OnDayPassed();

        /// <summary>
        /// Отрисовать сущность с учётом камеры.
        /// </summary>
        /// <param name="camera">Объект камеры для преобразования мировых координат в экранные.</param>
        public abstract void Draw(Systems.Camera camera);
    }
}
