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
        string activeLevel;
        delegate void EndCondition();
        EndCondition endCondition;
        void Level1()
        {
            game1.enemySpawnRate = 0.5f;
            game1.maxEnemies = 5;
            endCondition = delegate
            {
                if (Globals.HUD.pointsUI.Points >= 10)
                {
                    Level2();
                }
            };

        }
        void Level2()
        {
            game1.enemySpawnRate = 2f;
            game1.maxEnemies = 30;
            try
            {
                Globals.HUD.hotbar.Add(new Pistol(25, 0.1f, "mg", bulletVelocity: 1000f), 4);
            }
            catch { }
            endCondition = delegate
            {
                //if (Globals.HUD.pointsUI.Points >= 40)
                    //Level2();
            };
        }
        public void Update(GameTime gameTime)
        {
            endCondition();
        }
    }
}
