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
        //public int speed = 200;
        public Player(Texture2D texture)
        {
            Mass = 10;
            //Force = 10;
            Collision = true;
            Speed = 1/200f;
            //X = 100;
            //Y = 100;
            //Size = new Point(124, 124);
            Texture = texture;
            Hitbox = new Circle(100, 100, 200);
            //Hitbox = new Rect(X, Y, 64, 64);
            //Rect = new Rectangle((int)Hitbox.X, (int)Hitbox.Y, 124, 124);
        }

        public readonly Vector2 Scale 
        {
            get => new Vector2(0.5, 0.5);
        }

    }
}
