namespace Space_colony_game.World.Buildings
{
    /// <summary>
    /// Представляет фундамент — невозводимое, неразрушаемое основание для других зданий.
    /// </summary>
    public class Foundation : Building
    {
        /// <summary>Фундамент нельзя сносить.</summary>
        public override bool CanBeDestroyed => false;

        /// <summary>Создаёт экземпляр фундамента.</summary>
        public Foundation() { }

        /// <summary>Ежедневные эффекты для фундамента отсутствуют.</summary>
        public override void OnDayPassed() { }

        /// <summary>
        /// Рисует тело фундамента: пытается использовать текстуру, иначе рисует заливку цветом типа.
        /// </summary>
        /// <param name="x">Экранная X-позиция (пиксели).</param>
        /// <param name="y">Экранная Y-позиция (пиксели).</param>
        /// <param name="w">Ширина области отрисовки (пиксели).</param>
        /// <param name="h">Высота области отрисовки (пиксели).</param>
        public override void DrawBody(int x, int y, int w, int h)
            => DrawTextureOrFallback(Assets.Assets.TextureFoundation, x, y, w, h, Type.Color);
    }
}
