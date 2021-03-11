using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class Levels
    {
        Game1 game1;
        public Levels(Game1 game1)
        {
            this.game1 = game1;
            Level1();
        }
        delegate void EndCondition();
        EndCondition endCondition;
        void Level1()
        {
            Entity player = null;
            if(EntityTracker.GetEntities<Player>().Count > 0)
                player = EntityTracker.GetEntities<Player>()[0];

            Spawner slimeSpawner = new SlimeEnemySpawner(player);
            slimeSpawner.cooldown = 2f;
            slimeSpawner.maxEnemies = 5;
            slimeSpawner.type = "world";
            Globals.HUD.enemiesLeft = 5;
            Globals.HUD.currentWave = 1;
            endCondition = delegate
            {
                if (Globals.HUD.enemiesLeft <= 0)
                {
                    Level2();
                }
            };

        }
        void Level2()
        {
            RemoveWorldSpawners();
            Entity player = null;
            if (EntityTracker.GetEntities<Player>().Count > 0)
                player = EntityTracker.GetEntities<Player>()[0];

            Spawner enemySpawner = new EnemySpawner(player);
            enemySpawner.cooldown = 2f;
            enemySpawner.maxEnemies = 6;
            enemySpawner.type = "world";

            Spawner slimeSpawner = new SlimeEnemySpawner(player);
            slimeSpawner.cooldown = 5f;
            slimeSpawner.maxEnemies = 3;
            slimeSpawner.type = "world";

            Globals.HUD.currentWave = 2;
            Globals.HUD.enemiesLeft = 20;
            try
            {
                Globals.HUD.hotbar.Add(new Pistol(50, 1.5f, "sniper", bulletVelocity: 1500f));
                Globals.HUD.hotbar.Add(new Pistol(10, 0.1f, "mini"));
                //Globals.HUD.hotbar.Add(new Pistol(25, 0.1f, "mg", bulletVelocity: 1000f));
            }
            catch { }
            endCondition = delegate
            {
                if (Globals.HUD.enemiesLeft <= 0)
                    Level3();
            };
        }
        void Level3()
        {
            RemoveWorldSpawners();
            Entity player = null;
            if (EntityTracker.GetEntities<Player>().Count > 0)
                player = EntityTracker.GetEntities<Player>()[0];

            Spawner enemySpawner = new EnemySpawner(player);
            enemySpawner.cooldown = 2f;
            enemySpawner.maxEnemies = 15;
            enemySpawner.type = "world";

            Spawner slimeSpawner = new SlimeEnemySpawner(player);
            slimeSpawner.cooldown = 3f;
            slimeSpawner.maxEnemies = 10;
            slimeSpawner.type = "world";

            Globals.HUD.currentWave = 3;
            Globals.HUD.enemiesLeft = 100;
            try
            {
                Globals.HUD.hotbar.Add(new Pistol(50, 0.05f, "mg", bulletVelocity: 1500f));
            }
            catch { }
            endCondition = delegate
            {
                
            };
        }
        void RemoveWorldSpawners()
        {
            foreach (var o in Globals.Updatables)
            {
                if (o is Spawner)
                {
                    if ((o as Spawner).type == "world")
                        o.IsDead = true;
                }
            }
        }
        public void Update(GameTime gameTime)
        {
            endCondition();
        }
    }
}
