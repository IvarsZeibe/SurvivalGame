using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class Room
    {
        public (int x, int y) Coords { get; set; }
        public Color BackgroundColor { get; set; }
        public Drawing background { get; set; }
        public string Name { get; set; }
        public List<Entity> Entities = new List<Entity>();
        public List<Level> Levels = new List<Level>() {new Level("None", 0) };
        public Level ActiveLevel { get => Levels[activeLevelIndex]; }
        private int activeLevelIndex { get; set; } = 0;
        public bool CanLeave { get; set; } = true;
        public double windXCoord { get; set; } = 0;
        Room()
        {
        }
        public Room((int x, int y) coords, string name, Color color, TextureName backgroundTexture = TextureName.None, bool addToRooms = true)
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

            if (addToRooms)
            {
                Load();
                Globals.Rooms.Add(coords, this);
            }
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
            windXCoord += gameTime.ElapsedGameTime.TotalSeconds * Globals.rand.Next(100,800);
            if(windXCoord > Globals.rand.Next(5000, 40000))
            {
                windXCoord = 0;
            }

        }
        bool isActive = false;
        public void Load()
        {
            isActive = true;
            if (!(background is null))
                background.Enable();
            foreach (var e in Entities)
            {
                e.Load();
            }
            foreach (var level in Levels)
            {
                level.Enable();
            }
        }
        public void UnLoad()
        {
            isActive = false;
            List<Entity> deadEntities = new List<Entity>();
            foreach (var e in Entities)
            {
                if (e.IsDead)
                    deadEntities.Add(e);
                else
                    e.UnLoad();
            }
            foreach(var ent in deadEntities)
                Entities.Remove(ent);

            if (!(background is null))
                background.Disable();
            foreach (var level in Levels)
            {
                level.Disable();
            }
        }
    }
}
