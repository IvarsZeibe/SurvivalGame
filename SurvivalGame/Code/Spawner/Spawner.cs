using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    abstract class Spawner
    {
        public string type { get; set; } = "normal";
        public virtual float cooldown { get; set; } = 1f;
        public virtual float maxEnemies { get; set; } = 0f;
        public virtual int attempts { get; set; } = 10;
        public virtual float cooldownRandomization { get; set; } = 0;

        protected Random rand = Globals.rand;
        public float timeSinceSpawn { get; set; } = 0f;

        public Spawner() { }

        public void Update(GameTime gameTime)
        {
            timeSinceSpawn += (float)gameTime.ElapsedGameTime.TotalSeconds;
            cooldownRandomization = ((float)rand.NextDouble() - 0.5f) * 0.3f;
            if (timeSinceSpawn > cooldown + cooldownRandomization)
            {
                TrySpawn(gameTime);
            }
        }
        public virtual void TrySpawn(GameTime gameTime)
        {

        }
    }
}
