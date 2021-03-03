using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class NoBrainEntity: Entity
    {
        float timeTillDeath = 3;
        public NoBrainEntity(float? x = null, float? y = null)
        {
            Random rand = new Random();
            x ??= rand.Next(0, Globals.graphics.PreferredBackBufferWidth);
            y ??= rand.Next(0, Globals.graphics.PreferredBackBufferHeight);
            this.Hitbox = new Circle((double)x, (double)y, 0);
            this.Mass = 0;
            this.Collision = false;
            this.Speed = 0;
            this.Health = 100;
            this.MaxHealth = Health;
        }
        public override void Update(GameTime gameTime)
        {
            timeTillDeath -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timeTillDeath < 0)
                IsDead = true;
        }
    }
}
