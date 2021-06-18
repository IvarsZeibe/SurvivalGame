﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    class Level
    {
        public bool IsActive { get; set; } = true;
        public List<Spawner> spawners { get; set; } = new List<Spawner>();
        public string Name { get; set; }
        public int EnemiesLeft { get; set; }
        //Func<bool> EndCondition;
        Level() { }
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
