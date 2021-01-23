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
        public static List<Entity> Entities { get; } = new List<Entity>();
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
                    if (Entities[i].IsDead)
                        DeadEntities.Add(Entities[i]);
                    else
                        Entities[i].Update(gameTime);
                }
                foreach (var deadEntity in DeadEntities)
                {
                    Entities.Remove(deadEntity);
                }
                DeadEntities.Clear();
        }
        /// <summary>
        /// Creates new entitie
        /// </summary>
        public static class Add
        {
            public static Player Player()
            {
                Player player = new Player();
                Entities.Add(player);
                return player;
            }

            public static Enemy Enemy(TextureName texture, float x, float y, int width = 20, int height = 0, int speed = 100, bool collision = true, Entity target = null, Color? color = null)
            {
                Enemy enemy = new Enemy(texture, x, y, width, height, speed, collision, target, color);
                //Enemy enemy = new Enemy() { Texture = texture ??  };
                Entities.Add(enemy);
                //Enemies.Add(enemy);
                return enemy;
            }
            public static Projectile Projectile(TextureName texture, float speed, Vector2 source, Vector2 target, int damage)
            {
                Projectile projectile = new Projectile(texture, speed, source, target, damage);
                Entities.Add(projectile);
                //Projectiles.Add(projectile);
                return projectile;
            }
            public static Sword Sword(TextureName texture, Entity player, float rotation, int damage)
            {
                Sword sword = new Sword(texture, player, rotation, damage);
                Entities.Add(sword);
                //Swords.Add(sword);
                return sword;
            }
            
            public static Wall Wall(TextureName texture, double x, double y, bool collision = true)
            {
                Wall wall = new Wall(TextureName.Rectangle, x, y, collision);
                Entities.Add(wall);
                //Entities.Add(wall);
                return wall;
            }
            public static MouseCursor MouseCursor()
            {
                MouseCursor mouseCursor = new MouseCursor();
                Entities.Add(mouseCursor);
                return mouseCursor;
            }
        }
    }
}
