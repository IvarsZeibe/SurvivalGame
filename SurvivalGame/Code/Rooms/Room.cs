using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class Room
    {
        public readonly (int x, int y)Coords;
        public Color BackgroundColor { get; set; }
        public Drawing background;
        public string Name { get; set; }
        public List<Entity> Entities { get; set; } = new List<Entity>();
        public List<Level> Levels { get; set; } = new List<Level>() {new Level("None", 0) };
        public Level ActiveLevel { get => Levels[activeLevelIndex]; }
        private int activeLevelIndex = 0;
        public bool CanLeave = true;
        public Room((int x, int y) coords, string name, Color color, TextureName backgroundTexture = TextureName.None)
        {
            Name = name;
            Coords = coords;
            BackgroundColor = color;
            if (backgroundTexture is TextureName.None)
            {
                background = null;
            }
            else
            {
                background = new Drawing(backgroundTexture, Vector2.Zero, color, 0f,
                    new Vector2(Globals.graphics.PreferredBackBufferWidth, Globals.graphics.PreferredBackBufferHeight), 1f, false);
            }

            Load();
            Globals.Rooms.Add(coords, this);
        }
        public void Update(GameTime gameTime)
        {
            if (!Levels[activeLevelIndex].IsActive)
            {
                if (activeLevelIndex < Levels.Count - 1)
                    activeLevelIndex++;
                else activeLevelIndex = 0;
            }
            Levels[activeLevelIndex].Update(gameTime);

        }
        bool isActive = false;
        public void Load()
        {
            isActive = true;
            if (!(background is null))
                background.Enable();
            foreach(var level in Levels)
            {
                level.Enable();
            }
        }
        public void UnLoad()
        {
            isActive = false;
            foreach (var e in Entities)
            {
                e.UnLoad();
            }
            if (!(background is null))
                background.Disable();
            foreach (var level in Levels)
            {
                level.Disable();
            }
        }
    }
}
