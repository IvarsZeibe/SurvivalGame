using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    partial class HUD
    {
        public Hotbar hotbar;

        Dictionary<string, Drawing> drawings = new Dictionary<string, Drawing>();
        Dictionary<string, DrawingText> textDrawings = new Dictionary<string, DrawingText>();

        public int points { get; set; } = 0;
        public int currentWave { get; set; } = 0;
        int health
        {
            get => EntityTracker.GetEntities<Player>().Count > 0 ? EntityTracker.GetEntities<Player>()[0].Health : 0;
        }
        public int enemiesLeft { get; set; } = 0;

        public HUD()
        {
            CreatePointsUI();
            CreateWaveDisplayUI();
            CreateHealthUI();
            CreateEnemiesLeftUI();
            hotbar = new Hotbar();
        }
        public void Update(GameTime gameTime)
        {
            textDrawings["PointsUI"].Text = new StringBuilder(points.ToString());
            textDrawings["WaveDisplayUI"].Text = new StringBuilder("Wave: " + currentWave);
            textDrawings["HealthUI"].Text = new StringBuilder("Health: " + health);
            textDrawings["EnemiesLeftUI"].Text = new StringBuilder("Enemies left: " + enemiesLeft);
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
                    new Vector2(20, 20)));
            textDrawings.Add(
                "PointsUI",
               new DrawingText(
                    SpriteFontName.Aerial16,
                    new StringBuilder(points.ToString()),
                    new Vector2(50, Globals.graphics.PreferredBackBufferHeight - 100),
                    Color.White,
                    0,
                    new Vector2(1, 1),
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
                    isDrawn: true));
        }
        private void CreateEnemiesLeftUI()
        {
            textDrawings.Add("EnemiesLeftUI",
                new DrawingText(
                    SpriteFontName.Aerial16,
                    new StringBuilder("Enemies left: " + enemiesLeft),
                    new Vector2(20, Globals.graphics.PreferredBackBufferHeight - 190),
                    Color.White,
                    0,
                    new Vector2(1, 1),
                    isDrawn: true));
        }
    }
}
