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
        private Button editorButton;
        private Button closeButton;
        private Button loadButton;
        public bool IsActive;
        public MainMenu()
        {
            background = new Drawing(TextureName.Rectangle, Vector2.Zero, new Color(0,0,0,100), 0f, 
                new Vector2(Globals.graphics.PreferredBackBufferWidth, Globals.graphics.PreferredBackBufferHeight), 0.11f, true);
            startButton = new Button(
                new Rect(Globals.graphics.PreferredBackBufferWidth / 2, Globals.graphics.PreferredBackBufferHeight / 2, 100, 50),
                Color.Black, new StringBuilder("Start"), Color.White);
            editorButton = new Button(
                new Rect(Globals.graphics.PreferredBackBufferWidth / 2, Globals.graphics.PreferredBackBufferHeight / 2 + 55, 100, 50),
                Color.Black, new StringBuilder("Editor"), Color.White);
            closeButton = new Button(
                new Rect(Globals.graphics.PreferredBackBufferWidth / 2, Globals.graphics.PreferredBackBufferHeight / 2 + 110, 100, 50),
                Color.Black, new StringBuilder("Save and quit"), Color.White);
            loadButton = new Button(
                new Rect(Globals.graphics.PreferredBackBufferWidth / 2, Globals.graphics.PreferredBackBufferHeight / 2 + 165, 100, 50),
                Color.Black, new StringBuilder("Save and quit"), Color.White);
            Activate();
        }
        public void CheckClickEvent()
        {
            if (Globals.MouseCursor.Hitbox.CollidesWith(startButton.Hitbox))
            {
                Globals.HUD.Activate();
                Globals.gameActive = true;
                Globals.Editor.IsActive = false;
                Globals.getActiveRoom.Load();
                Deactivate();
            }
            if (Globals.MouseCursor.Hitbox.CollidesWith(editorButton.Hitbox))
            {
                Globals.gameActive = false;
                Globals.Editor.IsActive = true;
                Globals.getActiveRoom.UnLoad();
                Deactivate();
            }
            if (Globals.MouseCursor.Hitbox.CollidesWith(closeButton.Hitbox))
            {
                Utilities.SaveGame();
            }
            if (Globals.MouseCursor.Hitbox.CollidesWith(loadButton.Hitbox))
            {
                Utilities.LoadGame();
            }
        }
        public void Activate()
        {
            IsActive = true;
            startButton.Activate();
            editorButton.Activate();
            background.Enable();
            Globals.HUD.Deactivate();
        }
        public void Deactivate()
        {
            IsActive = false;
            startButton.Deactivate();
            editorButton.Deactivate();
            background.Disable();
        }
    }
}
