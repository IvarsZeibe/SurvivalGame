using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class DefaultLevels
    {
        public bool IsDone { get; set; } = false;
        Game1 game1;
        public DefaultLevels(Game1 game1)
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
            slimeSpawner.cooldown = 1f;
            slimeSpawner.maxEnemies = 5;
            slimeSpawner.type = "world";

            Globals.HUD.EnemiesLeft = 8;
            Globals.HUD.currentWave = 1;

            try
            {
                Globals.HUD.hotbar.Add(new SwordItem());
            }
            catch { }
            Globals.shop.AddItemForSale(new Pistol(50, 1.5f, "sniper", bulletVelocity: 1500f), 3);
            Globals.shop.AddItemForSale(new Pistol(10, 0.1f, "mini"), 5);
            Globals.shop.AddItemForSale(new SwordItem(40, knockbackStrenght: 5), 3);
            Globals.shop.AddItemForSale(new RPG(_trackEnemy: false), 5);

            endCondition = delegate
            {
                if (Globals.HUD.EnemiesLeft <= 0)
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
            Globals.HUD.EnemiesLeft = 20;

            Globals.shop.AddItemForSale(new RPG(), 15);
            endCondition = delegate
            {
                if (Globals.HUD.EnemiesLeft <= 0)
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
            Globals.HUD.EnemiesLeft = 30;
            try
            {
                Globals.HUD.hotbar.Add(new Pistol(50, 0.05f, "mg", bulletVelocity: 1500f));
            }
            catch { }
            endCondition = delegate
            {
                if (Globals.HUD.EnemiesLeft <= 0)
                    Done();
            };
        }
        void Done()
        {
            RemoveWorldSpawners();
            IsDone = true;
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
