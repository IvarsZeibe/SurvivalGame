using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class MainMenu
    {
        private Drawing background;
        private Button startButton;
        public bool IsActive;
        public MainMenu()
        {
            background = new Drawing(TextureName.Rectangle, Vector2.Zero, new Color(0,0,0,100), 0f, 
                new Vector2(Globals.graphics.PreferredBackBufferWidth, Globals.graphics.PreferredBackBufferHeight), 0.11f, true);
            startButton = new Button(
                new Rect(Globals.graphics.PreferredBackBufferWidth / 2, Globals.graphics.PreferredBackBufferHeight / 2, 100, 50),
                Color.Black, new StringBuilder("Start"), Color.White);
            Activate();
        }
        public void CheckClickEvent()
        {
            if (Globals.MouseCursor.Hitbox.CollidesWith(startButton.Hitbox))
            {
                Globals.HUD.Activate();
                Deactivate();
            }
        }
        public void Activate()
        {
            IsActive = true;
            startButton.Activate();
            background.IsDrawn = true;
            Globals.HUD.Deactivate();
        }
        public void Deactivate()
        {
            IsActive = false;
            startButton.Deactivate();
            background.IsDrawn = false;
            Globals.HUD.Activate();
        }
    }
}
