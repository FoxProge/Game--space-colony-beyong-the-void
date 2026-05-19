using Space_colony_game.Systems;

namespace Space_colony_game.World.Buildings
{
    /// <summary>
    /// Базовый класс для зданий, требующих/производящих энергию.
    /// Реализует общую логику потребления/наличия питания, ремонта и применения погодного урона.
    /// </summary>
    public abstract class PoweredBuilding : Building
    {
        /// <summary>Потребление энергии этим зданием в единицах за сутки.</summary>
        public int PowerPerDay => Type.PowerPerDay;

        /// <summary>Флаг наличия питания у здания в текущий день.</summary>
        public bool HasPower { get; set; } = true;

        /// <summary>
        /// Текущая эффективность здания, зависит от состояния здоровья.
        /// Возвращает 0 при разрушении, 0.5 при здоровье ≤ 50, и 1.0 в противном случае.
        /// </summary>
        public float Efficiency => Health switch
        {
            0 => 0f,
            <= 50 => 0.5f,
            _ => 1.0f
        };

        /// <summary>Признак, что здание полностью разрушено (Health ≤ 0).</summary>
        public bool IsBroken => Health <= 0;

        /// <summary>
        /// Вызывается при наступлении нового дня: если здание не разрушено и имеет питание,
        /// применяется производственная логика (<see cref="ApplyProduction"/>).
        /// </summary>
        public override void OnDayPassed()
        {
            if (IsBroken || !HasPower) return;
            ApplyProduction();
        }

        /// <summary>
        /// Применяет ежедневное производство / эффекты здания. Реализуется в подклассах.
        /// </summary>
        protected abstract void ApplyProduction();

        /// <summary>
        /// Производит частичный ремонт здания, списывая металл из колонии.
        /// </summary>
        /// <param name="colony">Колония, из ресурсов которой оплачивается ремонт.</param>
        /// <remarks>
        /// Ремонт восстанавливает до 10 единиц здоровья за раз и стоит фиксированную сумму металла.
        /// Если ресурсов в колонии недостаточно — ремонт не выполняется.
        /// </remarks>
        public void Repair(Colony colony)
        {
            if (Health >= 100) return;

            int missing = 100 - Health;
            int repairAmt = Math.Min(missing, 10);
            int metalCost = 5;

            if (colony.Metal.Value < metalCost) return;

            colony.Metal.Value -= metalCost;
            Health = Math.Min(100, Health + repairAmt);
        }

        /// <summary>
        /// Применяет влияние погодных условий на состояние здания — шанс получить урон зависит от типа погоды.
        /// </summary>
        /// <param name="weather">Текущий тип погоды.</param>
        /// <param name="rng">Генератор случайных чисел, используемый для определения события урона.</param>
        /// <remarks>
        /// Для шторма шанс урона ~25%, для метели ~50%. Солнечные панели получают более серьёзный урон в этих условиях.
        /// </remarks>
        public void ApplyWeatherDamage(WeatherType weather, Random rng)
        {
            bool isSolar = this is SolarPanel;

            switch (weather)
            {
                case WeatherType.Storm:
                    // 25% шанс урона
                    if (rng.Next(100) < 25)
                    {
                        int dmg = isSolar
                            ? rng.Next(20, 31)   // 20-30 для солнечной панели
                            : rng.Next(5, 11);  // 5-10 для остальных
                        Health = Math.Max(0, Health - dmg);
                    }
                    break;

                case WeatherType.Blizzard:
                    // 50% шанс урона
                    if (rng.Next(100) < 50)
                    {
                        int dmg = isSolar
                            ? rng.Next(40, 51)   // 40-50 для солнечной панели
                            : rng.Next(20, 31);  // 20-30 для остальных
                        Health = Math.Max(0, Health - dmg);
                    }
                    break;
            }
        }
    }
}
