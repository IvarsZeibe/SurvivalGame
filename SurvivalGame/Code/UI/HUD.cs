using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class HUD
    {
        public Hotbar hotbar;
        public bool IsActive { get; set; } = true;

        Dictionary<string, Drawing> drawings = new Dictionary<string, Drawing>();
        Dictionary<string, DrawingText> textDrawings = new Dictionary<string, DrawingText>();

        public int points { get; set; } = 0;
        public int currentWave { get; set; } = 0;
        int health
        {
            get => EntityTracker.GetEntities<Player>().Count > 0 ? EntityTracker.GetEntities<Player>()[0].Health : 0;
        }
        public int EnemiesLeft
        {
            get => Globals.Rooms[Globals.activeRoomCoords].ActiveLevel.EnemiesLeft;
            set
            {
                if (Globals.Rooms[Globals.activeRoomCoords].ActiveLevel.EnemiesLeft > 0)
                    Globals.Rooms[Globals.activeRoomCoords].ActiveLevel.EnemiesLeft = value;
            }
        }

        public HUD()
        {
            CreatePointsUI();
            CreateWaveDisplayUI();
            CreateHealthUI();
            CreateEnemiesLeftUI();
            CreateCoordsUI();
            CreateRoomNameUI();
            hotbar = new Hotbar();
        }
        public void Update(GameTime gameTime)
        {
            textDrawings["PointsUI"].Text = new StringBuilder(points.ToString());
            textDrawings["WaveDisplayUI"].Text = new StringBuilder("Wave: " + currentWave);
            textDrawings["HealthUI"].Text = new StringBuilder("Health: " + health);
            textDrawings["EnemiesLeftUI"].Text = new StringBuilder("Enemies left: " + EnemiesLeft);
            textDrawings["CoordsUI"].Text = new StringBuilder("X: " + Globals.activeRoomCoords.x + " Y: " + Globals.activeRoomCoords.y);
            textDrawings["RoomNameUI"].Text = new StringBuilder("Room: " + Globals.Rooms[Globals.activeRoomCoords].Name);
        }
        public void Activate()
        {
            IsActive = true;
            hotbar.Activate();
            foreach(var drawing in drawings)
            {
                drawing.Value.IsDrawn = true;
            }
            foreach (var text in textDrawings)
            {
                text.Value.IsDrawn = true;
            }
        }
        public void Deactivate()
        {
            IsActive = false;
            hotbar.Deactivate();
            foreach (var drawing in drawings)
            {
                drawing.Value.IsDrawn = false;
            }
            foreach (var text in textDrawings)
            {
                text.Value.IsDrawn = false;
            }
        }
        private void CreatePointsUI()
        {
            drawings.Add(
                "PointsUI",
               new Drawing(
                    TextureName.Circle,
                    new Vector2(20, Globals.graphics.PreferredBackBufferHeight - 100),
                    Color.Yellow,
                    0f,
                    new Vector2(20, 20),
                    0.1f));
            textDrawings.Add(
                "PointsUI",
               new DrawingText(
                    SpriteFontName.Aerial16,
                    new StringBuilder(points.ToString()),
                    new Vector2(50, Globals.graphics.PreferredBackBufferHeight - 100),
                    Color.White,
                    0,
                    new Vector2(1, 1),
                    0.1f,
                    isDrawn: true));
        }
        private void CreateWaveDisplayUI()
        {
            textDrawings.Add("WaveDisplayUI",
                new DrawingText(
                    SpriteFontName.Aerial16,
                    new StringBuilder("Wave: " + currentWave),
                    new Vector2(20, Globals.graphics.PreferredBackBufferHeight - 130),
                    Color.White,
                    0,
                    new Vector2(1, 1),
                    0.1f,
                    isDrawn: true));
        }
        private void CreateHealthUI()
        {
            textDrawings.Add("HealthUI",
                new DrawingText(
                    SpriteFontName.Aerial16,
                    new StringBuilder("Health: " + health),
                    new Vector2(20, Globals.graphics.PreferredBackBufferHeight - 160),
                    Color.White,
                    0,
                    new Vector2(1, 1),
                    0.1f,
                    isDrawn: true));
        }
        private void CreateEnemiesLeftUI()
        {
            textDrawings.Add("EnemiesLeftUI",
                new DrawingText(
                    SpriteFontName.Aerial16,
                    new StringBuilder("Enemies left: " + EnemiesLeft),
                    new Vector2(20, Globals.graphics.PreferredBackBufferHeight - 190),
                    Color.White,
                    0,
                    new Vector2(1, 1),
                    0.1f,
                    isDrawn: true));
        }
        private void CreateCoordsUI()
        {
            textDrawings.Add("CoordsUI",
                new DrawingText(
                    SpriteFontName.Aerial16,
                    new StringBuilder("X: " + Globals.activeRoomCoords.x + " Y: " + Globals.activeRoomCoords.y),
                    new Vector2(20, Globals.graphics.PreferredBackBufferHeight - 210),
                    Color.White,
                    0,
                    new Vector2(1, 1),
                    0.1f,
                    isDrawn: true));
        }
        private void CreateRoomNameUI()
        {
            textDrawings.Add("RoomNameUI",
                new DrawingText(
                    SpriteFontName.Aerial16,
                    new StringBuilder("Room: " + Globals.Rooms[Globals.activeRoomCoords].Name),
                    new Vector2(20, Globals.graphics.PreferredBackBufferHeight - 230),
                    Color.White,
                    0,
                    new Vector2(1, 1),
                    0.1f,
                    isDrawn: true));
        }
    }
}
