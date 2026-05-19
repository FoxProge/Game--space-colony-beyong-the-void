using Raylib_cs;

namespace Space_colony_game.UI.Panels
{
    /// <summary>
    /// Базовый абстрактный класс для всех панелей UI.
    /// Определяет общие свойства геометрии и стиля, а также контракт на отрисовку.
    /// </summary>
    public abstract class Panel
    {
        /// <summary>Прямоугольник области панели на экране.</summary>
        public abstract Rectangle Body { get; set; }

        /// <summary>Цвет фона панели.</summary>
        public abstract Color BackgroundColor { get; set; }

        /// <summary>Цвет рамки панели.</summary>
        public abstract Color BorderColor { get; set; }

        /// <summary>Толщина рамки панели в пикселях.</summary>
        public abstract float BorderThickness { get; set; }

        /// <summary>Рисует панель (фон, рамку и содержимое).</summary>
        public abstract void Draw();
    }
}
