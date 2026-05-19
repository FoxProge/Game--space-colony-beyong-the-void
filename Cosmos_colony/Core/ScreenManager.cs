using Space_colony_game.Screens;

namespace Space_colony_game.Core
{
    /// <summary>
    /// Управляет переходами между игровыми экранами.
    /// Поддерживает установку следующего экрана, обновление и отрисовку текущего.
    /// </summary>
    public class ScreenManager
    {
        /// <summary>Текущий активный экран.</summary>
        private IScreen? current_;

        /// <summary>Экран, на который будет совершён переход в следующем обновлении.</summary>
        private IScreen? next_;

        /// <summary>
        /// Запланировать переход на указанный экран.
        /// </summary>
        /// <param name="screen">Экран, на который нужно перейти. Не должен быть <c>null</c>.</param>
        public void GoTo(IScreen screen) => next_ = screen;

        /// <summary>
        /// Выполняет логику смены экрана (если запланирован переход) и обновляет текущий экран.
        /// </summary>
        public void Update()
        {
            if(next_ != null)
            {
                current_?.OnExit();
                current_ = next_;
                next_ = null;
                current_.OnEnter();
            }

            current_?.Update();
        }

        /// <summary>
        /// Вызывает отрисовку текущего экрана, если он задан.
        /// </summary>
        public void Draw() => current_?.Draw();
    }
}
