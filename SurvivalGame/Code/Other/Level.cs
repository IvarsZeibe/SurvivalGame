using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class Level
    {
        public bool IsActive { get; set; } = true;
        public List<Spawner> spawners = new List<Spawner>();
        public readonly string Name;
        public int EnemiesLeft;
        //Func<bool> EndCondition;
        public Level(string name, int enemiesLeft)
        {
            Name = name;
            EnemiesLeft = enemiesLeft;
            //EndCondition = endCondition;
        }
        public void Update(GameTime gameTime)
        {
            if (EnemiesLeft <= 0)
                Disable();

        }
        public void Disable()
        {
            IsActive = false;
            foreach (var spawner in spawners)
                spawner.IsDead = true;
        }
        public void Enable()
        {
            IsActive = true;
            foreach (var spawner in spawners)
                spawner.IsDead = false;
        }

    }
}
