using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace SurvivalGame
{
    interface IUpdate
    {
        void Update(GameTime gameTime);
        bool UpdateEnabled { get; set; }
        public bool IsDead { get; set; }
    }
}
