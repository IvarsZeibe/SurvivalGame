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
    }
}
