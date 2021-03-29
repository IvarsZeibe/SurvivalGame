using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    abstract class Spawner : IUpdate
    {
        public bool UpdateEnabled { get; set; } = true;
        public bool IsDead { get; set; } = false;
        public string type = "normal";

        public virtual float cooldown { get; set; } = 1f;
        public virtual float maxEnemies { get; set; } = 0f;
        public virtual int attempts { get; set; } = 10;
        public virtual float cooldownRandomization { get; set; } = 0;

        protected Random rand = Globals.rand;
        protected float timeSinceSpawn = 0f;

        public Spawner()
        {
            Globals.Updatables.Add(this);
        }

        public void Update(GameTime gameTime)
        {
            timeSinceSpawn += (float)gameTime.ElapsedGameTime.TotalSeconds;
            TrySpawn(gameTime);
        }
        protected virtual void TrySpawn(GameTime gameTime)
        {

        }
    }
}
