using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class EnemySpawner : Spawner
    {
        public Entity target { get; set; }
        EnemySpawner() { }
        public EnemySpawner(Entity target) : base()
        {
            this.target = target;
        }
        public override void TrySpawn(GameTime gameTime)
        {
            if (EntityTracker.GetEntities<Enemy>().Count < maxEnemies)
            {
                for (int i = 0; i < attempts; i++)
                {
                    Enemy enemy;
                    bool suitableSpot = true;
                    if (rand.Next(2) == 1)
                        enemy = new Enemy(TextureName.Circle,
                            rand.Next(0, Globals.graphics.PreferredBackBufferWidth),
                            rand.Next(0, Globals.graphics.PreferredBackBufferHeight),
                            rand.Next(15, 25), target: target, color: Color.DarkSlateGray);
                    else
                        enemy = new Enemy(TextureName.Rectangle,
                            rand.Next(0, Globals.graphics.PreferredBackBufferWidth),
                            rand.Next(0, Globals.graphics.PreferredBackBufferHeight),
                            rand.Next(15, 25), rand.Next(25, 35), target: target, color: Color.DarkGray);
                    Globals.getActiveRoom.Entities.Add(enemy);

                    foreach (var entity in EntityTracker.Entities)
                    {
                        if (enemy != entity && enemy.CollidesWith(entity))
                        {
                            suitableSpot = false;
                            break;
                        }
                    }
                    if (enemy.Hitbox.Distance(target.Hitbox) < 200)
                        suitableSpot = false;
                    if (!suitableSpot)
                    {
                        EntityTracker.Entities.Remove(enemy);
                        enemy.Kill();
                    }
                    else
                        break;
                }
                timeSinceSpawn = 0f;
            }
        }
    }
}
