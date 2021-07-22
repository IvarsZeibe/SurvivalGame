using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        public List<Entity> Entities { get; set; } = new List<Entity>();
        public List<Level> Levels { get; set; } = new List<Level>() {new Level("None", 0) };
        public Level ActiveLevel { get => Levels[activeLevelIndex]; }
        private int activeLevelIndex = 0;
        public bool CanLeave = true;
        public double windXCoord = 0;
        //public LightMap light = new LightMap();
        public Room() { }
        public Room((int x, int y) coords, string name, Color color, TextureName backgroundTexture = TextureName.None)
        {
            Name = name;
            Coords = coords;
            BackgroundColor = color;
            background = new Drawing(backgroundTexture, Vector2.Zero, color, 0f,
                new Vector2(Globals.graphics.PreferredBackBufferWidth, Globals.graphics.PreferredBackBufferHeight), 1f, false);

            Load();
            Globals.Rooms.Add(coords, this);
        }
        public void Update(GameTime gameTime)
        {
            Globals.worldTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
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
            //lightMap.Update(gameTime);
        }
        public bool isActive { get; set; } = false;
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
                e.UnLoad();
                if (e.IsDead)
                    deadEntities.Add(e);
            }
            foreach (var ent in deadEntities)
                Entities.Remove(ent);

            if (!(background is null))
                background.Disable();
            foreach (var level in Levels)
            {
                level.Disable();
            }
        }
        public bool AddToRoom(Entity entity)
        {
            if (Entities.Contains(entity))
                return false;
            Entities.Add(entity);
            entity.IsLoaded = isActive;
            return true;
        }
    }
}
