using Raylib_cs;
using Space_colony_game.Core;
using Space_colony_game.Systems;
using Space_colony_game.UI;
using Space_colony_game.UI.Menus;
using Space_colony_game.UI.Panels;
using Space_colony_game.World;
using Space_colony_game.World.Buildings;
using System.Numerics;

namespace Space_colony_game.Screens
{
    public class LevelOne : GameScreen
    {
        private BuildingManager buildings_ = null!;
        private BuildMode buildMode_ = null!;
        private Engineer engineer_ = null!;
        private Scientist scientist_ = null!;
        private Logist logist_ = null!;
        private SelectionManager selection_ = null!;
        private ContextMenu contextMenu_ = null!;
        private MotherShip motherShip_ = null!;
        private Building? lastRMBClicledBuilding_ = null!;
        private ObjectivesPanel objectivesPanel_ = null!;
        private bool paused_ = false;

        protected override int[] WinCondition { get; set; } = [250, 250, 150];
        protected override int LoseCondition { get; set; } = 150;

        public LevelOne(ScreenManager screenManager, int index)
            : base(screenManager, index)
        {
            WinScore = false;
            LoseScore = false;
        }

        public override void OnEnter()
        {
            Assets.Assets.LoadTextures(0);
            buildings_ = new BuildingManager(World_map, colony_);
            miniMap_.Buildings = buildings_;
            buildMode_ = new BuildMode(buildings_, camera);
            engineer_ = new Engineer(colony_, buildMode_);
            scientist_ = new Scientist(colony_);
            logist_ = new Logist(colony_);
            motherShip_ = buildings_.SpawnMotherShip();
            selection_ = new SelectionManager(buildings_, camera)
            {
                OnMotherShipRightClick = (mousePos) =>
                {
                    contextMenu_.OpenForSpaceShip(mousePos);
                },

                OnBuildingRightClick = (building, mousePos) =>
                {
                    bool canRepair = building.Health < 100;
                    bool canDestroy = building.CanBeDestroyed;
                    contextMenu_.OpenForBuilding(mousePos, canRepair, canDestroy);
                    lastRMBClicledBuilding_ = building;
                }
            };
            contextMenu_ = new ContextMenu
            {
                OnSelectSpecialist = (idx) => OpenSpecialistPanel(idx),

                OnRepair = () =>
                {
                    if (lastRMBClicledBuilding_ is PoweredBuilding pb)
                        pb.Repair(colony_);
                },

                OnDestroy = () =>
                {
                    if (lastRMBClicledBuilding_ != null)
                    {
                        buildings_.TryDestroy(lastRMBClicledBuilding_.Id);
                        selection_.Deselect();
                        lastRMBClicledBuilding_ = null;
                    }
                }
            };
            colony_.Buildings = buildings_;
            OnNewDay = () => Transition.Start(colony_.Day + 1);
            OnOpenSpecialist = (idx) =>
            {
                if (idx == 0)
                    engineer_.Open();
                else if (idx == 1)
                    scientist_.Open();
                else if (idx == 2)
                    logist_.Open();
            };
            Transition.OnMidnight = () =>
            {
                colony_.ProcessDay();
                logist_.OnDayPassed();
            };
            camera.CenterOn(
                new Vector2(
                    (motherShip_.Col + MotherShip.Size / 2f) * WorldMap.TileSize,
                    (motherShip_.Row + MotherShip.Size / 2f) * WorldMap.TileSize
                )    
            );
            objectivesPanel_ = new(
                colony_, WinCondition,
                LoseCondition, new Rectangle(
                    miniMap_.Body.X, miniMap_.Body.Y + Game.ScaleInt(255),
                    miniMap_.Body.Width, 165
                )
            );
        }
        public override void OnExit() => Assets.Assets.UnloadTextures();
        public override void Update()
        {
            if (WinScore)
                screenManager_.GoTo(new WinScreen(screenManager_, colony_.Day));

            if (LoseScore)
                screenManager_.GoTo(new LoseScreen(screenManager_, colony_.Day));

            if(Raylib.IsKeyPressed(KeyboardKey.Backspace))
                paused_ = !paused_;

            if(paused_)
            {
                UpdatePauseMenu();
                return;
            }

            Transition.Update();
            if (!Transition.IsPlaying)
            {
                contextMenu_.Update();
                engineer_.Update();
                scientist_.Update();
                logist_.Update();

                if (!buildMode_.IsActive
                  && !engineer_.IsOpen
                  && !contextMenu_.IsOpen
                  && !scientist_.IsOpen
                  && !logist_.IsOpen)
                {
                    selection_.Update();
                }

                if (!engineer_.IsOpen
                  && !scientist_.IsOpen
                  && !logist_.IsOpen
                  && !contextMenu_.IsOpen
                  && !buildMode_.IsActive)
                {
                    camera.Update();
                }
                else if (buildMode_.IsActive)
                {
                    camera.UpdateKeyboardOnly();
                }

                miniMap_.Update();
                buildMode_.Update();
            }
            base.Update();

            if (colony_.Food.Value >= WinCondition[0]
              && colony_.Metal.Value >= WinCondition[1]
              && colony_.People.Value >= WinCondition[2]
              && colony_.Day <= LoseCondition)
            {
                LoseScore = false;
                WinScore = true;
            }
            else if (colony_.People.Value == 0
              || colony_.Day == LoseCondition)
            {
                LoseScore = true;
                WinScore = false;
            }
        }

        public override void Draw()
        {
            World_map.Draw(camera);
            buildings_.Draw(camera);
            selection_.Draw();
            buildMode_.Draw();
            DrawHUD();
            miniMap_.Draw();
            objectivesPanel_.Draw();
            engineer_.Draw();
            scientist_.Draw();
            logist_.Draw();
            contextMenu_.Draw();
            Transition.Draw();

            if (paused_)
                DrawPauseMenu();
        }

        private void DrawPauseMenu()
        {
            Raylib.DrawRectangle(
                0,
                0,
                Game.ScreenWidth,
                Game.ScreenHeight,
                new Color(0, 0, 0, 180));

            DrawPauseText(
                "PAUSED",
                -80,
                Assets.Assets.FontLargeSize);

            DrawPauseText(
                "ESC - продолжить",
                20,
                Assets.Assets.FontMediumSize);

            DrawPauseText(
                "ENTER - выйти в меню",
                70,
                Assets.Assets.FontMediumSize);
        }

        private void DrawPauseText(
            string text,
            int offsetY,
            float size)
        {
            Vector2 textSize = Raylib.MeasureTextEx(
                Assets.Assets.FontMedium,
                text,
                size,
                1);

            Raylib.DrawTextEx(
                Assets.Assets.FontMedium,
                text,
                new Vector2(
                    (Game.ScreenWidth - textSize.X) / 2,
                    Game.ScreenHeight / 2 + offsetY),
                size,
                1,
                Color.White);
        }

        public override void DrawBgImage() { }

        private void OpenSpecialistPanel(int idx)
        {
            switch (idx)
            {
                case 0: engineer_.Open(); break;
                case 1: scientist_.Open(); break;
                case 2: logist_.Open(); break;
            }
        }

        private void UpdatePauseMenu()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Enter))
                screenManager_.GoTo(new MainMenuScreen(screenManager_));
        }
    }
}
