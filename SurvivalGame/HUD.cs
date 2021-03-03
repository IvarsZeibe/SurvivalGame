using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    partial class HUD
    {
        public PointsUI pointsUI;
        public Hotbar hotbar;

        public HUD()
        {
            pointsUI = new PointsUI();
            hotbar = new Hotbar();
        }
        public class PointsUI
        {
            Drawing Drawing { get; set; }
            DrawingText PointsDrawing { get; set; }
            public int Points { get; set; } = 0;
            public PointsUI()
            {
                Drawing = new Drawing(
                    TextureName.Circle,
                    new Vector2(20, Globals.graphics.PreferredBackBufferHeight - 100),
                    Color.Yellow,
                    0f,
                    new Vector2(20, 20));
                Globals.Drawings.Add(Drawing);

                PointsDrawing = new DrawingText(
                    SpriteFontName.Aerial16,
                    new StringBuilder(Points.ToString()),
                    new Vector2(50, Globals.graphics.PreferredBackBufferHeight - 100),
                    Color.White,
                    0,
                    new Vector2(1,1),
                    isDrawn: true
                    );
                Globals.DrawingTexts.Add(PointsDrawing);
            }
            public void Update(GameTime gameTime)
            {
                PointsDrawing.Text = new StringBuilder(Points.ToString());
            }
        }
        public void Update(GameTime gameTime)
        {
            pointsUI.Update(gameTime);
        }
    }
}
