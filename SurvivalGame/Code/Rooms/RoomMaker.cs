using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class RoomMaker
    {
        //color = new Color(0, 100, 155, 200); night color

        public static Room RandomEmptyRoom((int y, int x) coords)
        {
            Color color = Color.White;
            TextureName texture = TextureName.None;
            List<Entity> entities = new List<Entity>();
            switch (Globals.rand.Next(1))
            {
                case 0:
                    color = new Color(0, 195, 30);
                    texture = TextureName.GrassyBackground;
                    break;
                //case 1:
                //    color = new Color(255,165,0,200);
                //    texture = TextureName.GrassyBackground;
                //    break;
                //case 2:
                //    color = new Color(128, 200, 128, 200);
                //    texture = TextureName.GrassyBackground;
                //    break;
                //case 3:
                //    color = new Color(208, 88, 28, 120);
                //    texture = TextureName.GrassyBackground;
                //    break;
            }
            var room = new Room(coords, "Empty room", color, texture);
            room.Entities.AddRange(entities);
            return room;
        }
        public static Room CustomRoom((int y, int x) coords, List<Entity> entities)
        {
            Color color = new Color(255, 165, 0, 200);
            TextureName texture = TextureName.GrassyBackground;
            List<Entity> Entities = new List<Entity>(entities);
            var room = new Room(coords, "Custom room", color, texture);
            room.Entities.AddRange(Entities);
            return room;
        }
        public static Room SlimeRoom((int y, int x) coords)
        {
            Color color = new Color(0, 135, 0);
            TextureName texture = TextureName.GrassyBackground;
            List<Entity> Entities = new List<Entity>();
            for (int i = 0; i < 20; i++)
            {
                Entities.Add(new Tree(Globals.rand.Next(0, Globals.graphics.PreferredBackBufferWidth),
                    Globals.rand.Next(0, Globals.graphics.PreferredBackBufferHeight)));
            }
            for (int i = 0; i < 6; i++)
            {
                Entity targ = new NoBrainEntity();
                Entities.Add(new SlimeEnemy(
                    Globals.rand.Next(0, Globals.graphics.PreferredBackBufferWidth), 
                    Globals.rand.Next(0, Globals.graphics.PreferredBackBufferHeight), targ, false));
                Entities.Add(targ);
            }
            for (int i = 0; i < 400; i++)
            {
                Vector2 pos = new Vector2(Globals.rand.Next(Globals.graphics.PreferredBackBufferWidth), Globals.rand.Next(Globals.graphics.PreferredBackBufferHeight));
                var grass = new Grass(pos);
                Entities.Add(grass);
            }
            var level = new Level("Wave1", 10);
            Entity target = new NoBrainEntity();
            Entities.Add(target);
            Spawner spawner = new SlimeEnemySpawner(target);
            spawner.maxEnemies = 6;
            level.spawners.Add(spawner);

            var room = new Room(coords, "Slime room", color, texture);
            room.Levels.Add(level);
            room.Entities.AddRange(Entities);
            return room;
        }
        public static Room ShooterRoom((int y, int x) coords)
        {
            Color color = new Color(235, 180, 120);
            TextureName texture = TextureName.GrassyBackground;
            //var g = Globals.Textures[TextureName.GrassyBackground.ToString()].;
            List<Entity> Entities = new List<Entity>();
            for (int i = 0; i < 2; i++)
            {
                Entity targ = new NoBrainEntity();
                Entities.Add(new Enemy(
                    TextureName.Rectangle,
                    Globals.rand.Next(0, Globals.graphics.PreferredBackBufferWidth),
                    Globals.rand.Next(0, Globals.graphics.PreferredBackBufferHeight),
                    target: targ, color: Color.DarkSlateGray, addToRoom: false));
                Entities.Add(targ);
            }
            for (int i = 0; i < 2; i++)
            {
                Entity targ = new NoBrainEntity();
                Entities.Add(new Enemy(
                    TextureName.Circle,
                    Globals.rand.Next(0, Globals.graphics.PreferredBackBufferWidth),
                    Globals.rand.Next(0, Globals.graphics.PreferredBackBufferHeight),
                    target: targ, color: Color.DarkGray, addToRoom: false));
                Entities.Add(targ);
            }
            for (int i = 0; i < 10; i++)
            {
                Vector2 pos = new Vector2(Globals.rand.Next(Globals.graphics.PreferredBackBufferWidth), Globals.rand.Next(Globals.graphics.PreferredBackBufferHeight));
                var stone = new Stone(pos);
                Entities.Add(stone);
            }
            var level = new Level("Wave1", 6);
            Entity target = new NoBrainEntity();
            Spawner spawner = new EnemySpawner(target);
            Entities.Add(target);
            spawner.maxEnemies = 6;
            level.spawners.Add(spawner);

            var room = new Room(coords, "Shooter room", color, texture);
            room.Levels.Add(level);
            room.Entities.AddRange(Entities);
            return room;
        }
        public static Room BossRoom((int y, int x) coords)
        {
            var room = new Room(coords, "Boss room", Color.DarkRed, TextureName.GrassyBackground);
            List<Entity> Entities = new List<Entity>() { new Boss() };
            room.Entities.AddRange(Entities);
            room.Levels.Add(new Level("Boss fight", 1));
            return room;
        }
    }
}
