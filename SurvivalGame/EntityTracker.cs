using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class EntityTracker
    {
        /// <summary>
        /// Entities list
        /// Sub lists
        /// </summary>

        public static List<IDrawing> Drawings { get; } = new List<IDrawing>();
        public static List<IDrawingText> DrawingsText { get; } = new List<IDrawingText>();
        public static List<IUpdate> ObjectsWithUpdate { get; } = new List<IUpdate>();

        public static List<Entity> Entities { get; } = new List<Entity>();
        public static List<Chat> UI { get; } = new List<Chat>();
        public static List<T> GetEntities<T>() where T : Entity
        {
            return Entities.FindAll(delegate (Entity entity) { return entity is T; }).ConvertAll(element => element as T);
        }
        /// <summary>
        /// Update
        /// </summary>
        private static List<Entity> DeadEntities { get; set; } = new List<Entity>();
        public static void UpdateEntities(GameTime gameTime)
        {
                for (int i = 0; i < Entities.Count; i++)
                {
                    Entities[i].Update(gameTime);
                    if (Entities[i].IsDead)
                        DeadEntities.Add(Entities[i]);

                }
                foreach (var deadEntity in DeadEntities)
                {
                    Entities.Remove(deadEntity);
                }
                DeadEntities.Clear();
            for (int i = 0; i < ObjectsWithUpdate.Count; i++)
            {
                ObjectsWithUpdate[i].Update(gameTime);
            }

        }
        /// <summary>
        /// Creates new entitie
        /// </summary>
        public static class Add
        {
            public static Player Player(Texture2D texture)
            {
                Player player = new Player(texture);
                Entities.Add(player);
                return player;
            }

            public static Enemy Enemy(Texture2D texture, float x, float y, int width = 20, int height = 0, int speed = 100, bool collision = true, Entity target = null, Color? color = null)
            {
                Enemy enemy = new Enemy(texture, x, y, width, height, speed, collision, target, color);
                //Enemy enemy = new Enemy() { Texture = texture ??  };
                Entities.Add(enemy);
                //Enemies.Add(enemy);
                return enemy;
            }
            public static Projectile Projectile(Texture2D texture, float speed, Vector2 source, Vector2 target, int damage)
            {
                Projectile projectile = new Projectile(texture, speed, source, target, damage);
                Entities.Add(projectile);
                //Projectiles.Add(projectile);
                return projectile;
            }
            public static Sword Sword(Texture2D texture, Entity player, float rotation)
            {
                Sword sword = new Sword(texture, player, rotation);
                Entities.Add(sword);
                //Swords.Add(sword);
                return sword;
            }
            
            public static Wall Wall(Texture2D texture, double x, double y, bool collision = true)
            {
                Wall wall = new Wall(texture, x, y, collision);
                Entities.Add(wall);
                //Entities.Add(wall);
                return wall;
            }
            public static MouseCursor MouseCursor(Texture2D texture)
            {
                MouseCursor mouseCursor = new MouseCursor(texture);
                Entities.Add(mouseCursor);
                return mouseCursor;
            }
        }
    }
}
