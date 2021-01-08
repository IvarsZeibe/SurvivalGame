using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalGame
{
    class Player : Entity
    {
        public Player(Texture2D texture)
        {
            Mass = 10;
            Collision = true;
            Speed = 1 / 200f;
            Texture = texture;
            Hitbox = new Circle(100, 100, 100);
            //drawing = Hitbox;
            LayerDepth = 0.1f;
        }
    }
}
