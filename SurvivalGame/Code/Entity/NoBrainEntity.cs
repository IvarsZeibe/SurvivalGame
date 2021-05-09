﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class NoBrainEntity: Entity
    {
        float timeTillDeath;
        public NoBrainEntity(float? x = null, float? y = null, float timeTillDeath = 3)
        {
            Random rand = new Random();
            x ??= rand.Next(0, Globals.graphics.PreferredBackBufferWidth);
            y ??= rand.Next(0, Globals.graphics.PreferredBackBufferHeight);
            this.timeTillDeath = timeTillDeath;
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
        public override void Load() { }
        public override void UnLoad() { }
    }
}