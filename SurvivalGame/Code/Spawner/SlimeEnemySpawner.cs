using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class SlimeEnemySpawner : Spawner
    {
        Entity target;
        public SlimeEnemySpawner(Entity target) : base()
        {
            this.target = target;
        }
        protected override void TrySpawn(GameTime gameTime)
        {
            cooldownRandomization = ((float)rand.NextDouble() - 0.5f)*0.3f;
            if(timeSinceSpawn > cooldown + cooldownRandomization)
            {
                if (EntityTracker.GetEntities<SlimeEnemy>().Count < maxEnemies)
                {
                    for (int i = 0; i < attempts; i++)
                    {
                        SlimeEnemy slime;
                        bool suitableSpot = true;
                        slime = new SlimeEnemy(
                            rand.Next(0, Globals.graphics.PreferredBackBufferWidth),
                            rand.Next(0, Globals.graphics.PreferredBackBufferHeight),
                            target);
                        foreach (var e in EntityTracker.Entities)
                        {
                            if (slime != e && slime.CollidesWith(e))
                            {
                                suitableSpot = false;
                                break;
                            }
                        }
                        if (slime.Hitbox.Distance(target.Hitbox) < 200)
                            suitableSpot = false;
                        if (!suitableSpot)
                        {
                            EntityTracker.Entities.Remove(slime);
                            slime.Kill();
                        }
                        else
                            break;
                    }
                    timeSinceSpawn = 0f;
                }

            }
        }
    }
}
